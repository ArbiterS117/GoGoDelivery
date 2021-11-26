using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public LayerMask GroundLayer;
    public LayerMask RailLayer;

    public Rigidbody2D RigidBody = null;

    public Vector2 velocityMax;
    public Vector2 velocityMin;

    //private static const JUMPFORCE = 5.0f;
    public float Jumpforce = 5.0f;
    public bool IsGround = false;
    public float RaycastDistance = 2.5f;

    public float AddSpeed = 1.0f;
    public float AddRailSpeed = 2.0f;

    public float Speed;
    //private int Dir = 1;            // 右
    public float MaxMoveSpeed = 10.0f;
    public float Decelerate = 0.6f;

    public bool OnRail = false;
    public bool OnRailPre = false;

    public float OnRailSpeed = 30.0f;
    public bool OnRailDir = true;// 1: Right 0: Left

    public float OnRailUpOffset = 2.0f;

    public float MaxOnRailSpeed = 40.0f;
    public float MinOnRailSpeed = 20.0f;

    public Vector2 RailNormal　= new Vector2(0.0f, 1.0f);
    public float ExitRailJumpForce = 25.0f;
    public float ExitCurvedRailJumpForce = 50.0f;
    public bool OnRailJumpTrigger = false;

    //=========================== game
    public int score = 0;
    //public bool HoldingNimotu = false;

    public int HoldingNimotsuNum = 0;
    static public int MaxNimotsuNum = 3;

    public Transform[] StartRebornPoint;
    public Transform[] RebornPoint;
    public bool[] RebornPointUsed;
    public Transform[] CRebornPoint;
    public bool[] CRebornPointUsed;
    public int RebornPointNum;

    public GameObject[] NimotsuHolded = new GameObject[MaxNimotsuNum];

    float oriGravity = 0.0f;

    //=========================== grapple rail
    static public int MaxGRailNum = 3;
    public int usedGrail = 0;
    public List<GameObject> GRail = new List<GameObject>();
    //public GameObject[] GRail = new GameObject[MaxGRailNum];

     // Start is called before the first frame update

     void Start()
    {
        RigidBody = this.GetComponent<Rigidbody2D>();
        Speed = 0.0f;

        //GameObject x, y, z;
        for (int i = 0; i < 3; i++)
        {
            NimotsuHolded[i] = null;
        }

        oriGravity = RigidBody.gravityScale;
        RebornPointNum = RebornPoint.Length;
    }

    private void FixedUpdate()
    {
        if (!OnRail) PhysicsUpdate();
        else OnRailPhysicsUpdate();

        OnRailPre = OnRail;

        //速度制限
        float vx = Mathf.Clamp(RigidBody.velocity.x, velocityMin.x, velocityMax.x);
        float vy = Mathf.Clamp(RigidBody.velocity.y, velocityMin.y, velocityMax.y);
        RigidBody.velocity = new Vector2(vx, vy);
    }

    // Update is called once per frame
    void Update()
    {
        IsGround = IsGrounded();   
        OnRailCheck();
        if (OnRail) RigidBody.gravityScale = 0.0f;
        else RigidBody.gravityScale = oriGravity;

        ///============================================================
        if (!OnRail)
            InputUpdate();
        else
            OnRailInputUpdate();

        StatusUpdate();

        playerSub.AnimationUpdate(this, GetComponent<Animator>());
        playerSub.GrappleUpdate(this);
        //PhysicsUpdate();


        // 下坡修正
        //RaycastHit2D hit3 = Physics2D.Raycast(transform.position, Vector2.down, RailLayer);
        //if (hit3 == false) { }
        //else
        //{
        //    transform.position = hit3.point;
        //    transform.position = new Vector3(transform.position.x, transform.position.y + RailUpOffSet, transform.position.z);
        //}        
    }

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down * RaycastDistance;

        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, RaycastDistance, GroundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    void InputUpdate()
    {
        if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow)) //左
        {
            this.Speed -= AddSpeed;
            OnRailDir = false;
        }
        if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) //右
        {
            this.Speed += AddSpeed;
            OnRailDir = true;

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGround) 
            {
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.1f);
                RigidBody.AddForce(new Vector2(0, Jumpforce));
            }
           
        }
    }

    void StatusUpdate()
    {
        if (IsGround)
        {
            usedGrail = 0;
        }
    }

    void PhysicsUpdate()
    {
        if (Speed > MaxMoveSpeed) Speed = MaxMoveSpeed;
        if (Speed < -MaxMoveSpeed) Speed = -MaxMoveSpeed;

        this.transform.Translate(Vector3.right * Speed * Time.deltaTime);

        if (Speed <= Decelerate * Time.deltaTime && Speed > 0) Speed = 0;
        else if (Speed > 0) Speed -= Decelerate * Time.deltaTime;

        if (Speed >= -Decelerate * Time.deltaTime && Speed < 0) Speed = 0;
        else if (Speed < 0) Speed += Decelerate * Time.deltaTime;

        //Exit Rail
        if (OnRailPre)
        {
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.0f);
            //jump
            if (((Speed >= 0.0f && RailNormal.x < -0.7f) || (Speed < 0.0f && RailNormal.x > 0.7f) ) && !OnRailJumpTrigger)
            {
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, ExitCurvedRailJumpForce);
            }
            else if (((Speed >= 0.0f && RailNormal.x < -0.2f) || (Speed < 0.0f && RailNormal.x > 0.2f)))
            {
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, ExitRailJumpForce);
            }
        }

    }

    void OnRailCheck()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down /** RaycastDistance*/;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, RaycastDistance, RailLayer);

        //rot reset
        this.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

        if (hit.collider != null)
        {
            if (RigidBody.velocity.y <= 0.0f)
            {
                OnRail = true;
                OnRailJumpTrigger = false;
            }

            if (OnRail == true)
            {
                //Rect 角
                if(Vector2.Dot(RailNormal,hit.normal) <= 0.2f)
                {
                    OnRail = false;
                    return;
                }

                RailNormal = hit.normal;


                //pos & rot
                Vector3 pos = transform.position;
                Vector2 UpOffset = hit.normal * OnRailUpOffset;
                this.transform.position = new Vector3(hit.point.x , hit.point.y + OnRailUpOffset, pos.z);

                Quaternion rot = GetComponent<Transform>().rotation;
                Vector2 up = new Vector2(0, 1);
                var angle = Vector2.Angle(up, hit.normal);
                if (hit.normal.x <= 0.0f) this.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, angle));
                else this.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, -angle));
            }


        }
        else OnRail = false;
    }

    void OnRailInputUpdate()
    {
        if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow)) //左
        {
            this.Speed -= AddRailSpeed;
        }
        if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) //右
        {
            this.Speed += AddRailSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (!IsGround) return;
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.1f);
            RigidBody.AddForce(new Vector2(0, Jumpforce));
            OnRail = false;
            OnRailJumpTrigger = true;
        }
    }
    void OnRailPhysicsUpdate()
    {
        if (OnRailDir)
        {
            if (Speed > MaxOnRailSpeed) Speed = MaxOnRailSpeed;
            if (Speed < MinOnRailSpeed) Speed = MinOnRailSpeed;
        }
        else
        {
            if (Speed < -MaxOnRailSpeed) Speed = -MaxOnRailSpeed;
            if (Speed > -MinOnRailSpeed) Speed = -MinOnRailSpeed;
        }
        this.transform.Translate(Vector3.right * Speed * Time.deltaTime);

        //if (Speed <= Decelerate && Speed > 0) Speed = 0;
        //else if (Speed > 0) Speed -= Decelerate;

        //if (Speed >= -Decelerate && Speed < 0) Speed = 0;
        //else if (Speed < 0) Speed += Decelerate;
    }

    //  当たり判定
    private void OnTriggerEnter2D(Collider2D other)
    {
        //if(other.tag == "Platform")
        //{
        //    if(other.transform.rotation.z != 0.0f /*&& other.transform.rotation.z <= 45.0f*/)
        //    {
        //        if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow)) //左
        //        {
        //            RigidBody.AddForce(new Vector2(-ClimbForce, 0));
        //        }
        //        if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) //右
        //        {
        //            RigidBody.AddForce(new Vector2(ClimbForce, 0));
        //        }
        //    }
        //}

        if(other.tag == "Item")
        {
            if (HoldingNimotsuNum < MaxNimotsuNum)
            {
                for(int i = 0; i < 3; i++)
                {
                    if (NimotsuHolded[i] == other.gameObject) return;
                }

                for (int i = 0; i < 3; i++)
                {
                    if (NimotsuHolded[i] == null)
                    {
                        NimotsuHolded[i] = other.gameObject;
                        other.GetComponent<Renderer>().enabled = false;
                        HoldingNimotsuNum++;

                        break;
                    }
                }

                //Destroy(other.gameObject);

            }
        }

        if(other.tag == "customer")
        {
            if (HoldingNimotsuNum > 0)
            {
                for(int i = 0; i < HoldingNimotsuNum; i++)
                {
                    if(NimotsuHolded[i].GetComponent<Nimotu>().GGGoal
                        == other.gameObject)
                    {
                        HoldingNimotsuNum--;
                        Destroy(other.gameObject);
              
                        Destroy(NimotsuHolded[i].gameObject);
                        NimotsuHolded[i] = null;

                        //ArrowCtrl.IsArrowed[i] = false;

                        score++;

                        //-------------------------生成----------------------------
                        // 新しいItem生成
                        GameObject obj = (GameObject)Resources.Load("Nimotu");
                        GameObject Item = (GameObject)Instantiate(obj,
                                                      RebornPoint[Random.Range(0, 9 + 1)].position,
                                                      Quaternion.identity);

                        // 新しい目標生成
                        GameObject xxx = (GameObject)Resources.Load("customer");
                        GameObject Goal = (GameObject)Instantiate(xxx,
                                                      CRebornPoint[Random.Range(0, 9 + 1)].position,
                                                      Quaternion.identity);
                        // itemと目標を連結
                        Item.GetComponent<Nimotu>().GGGoal = Goal;

                        break;
                    }
                }
                //HoldingNimotsuNum--;
                //HoldingNimotu = false;
            }
        }
    }

}

