using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public LayerMask GroundLayer;
    public LayerMask RailLayer;

    public Rigidbody2D RigidBody = null;

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
    public bool OnRailDir = true;

    public float MaxOnRailSpeed = 40.0f;
    public float MinOnRailSpeed = 20.0f;

    //=========================== game
    public int score = 0;
    //public bool HoldingNimotu = false;

    public int HoldingNimotsuNum = 0;
    static private int MaxNimotsuNum = 3;

    public Transform[] RebornPoint;
    public Transform[] CRebornPoint;

    public GameObject[] NimotsuHolded = new GameObject[3];

    // Start is called before the first frame update

    void Start()
    {
        RigidBody = this.GetComponent<Rigidbody2D>();
        Speed = 0.0f;

        //GameObject x, y, z;
        for(int i = 0; i < 3; i++)
            NimotsuHolded[i] = null;
    }

    private void FixedUpdate()
    {
        if (!OnRail) PhysicsUpdate();
        else OnRailPhysicsUpdate();

        OnRailPre = OnRail;
    }

    // Update is called once per frame
    void Update()
    {
        IsGround = IsGrounded();
        OnRailCheck();
        ///============================================================
        if (!OnRail)
            InputUpdate();
        else
            OnRailInputUpdate();

        StatusUpdate();

        playerSub.AnimationUpdate(this, GetComponent<Animator>());
        //PhysicsUpdate();
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
        }
        if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) //右
        {
            this.Speed += AddSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGround) 
            {
                RigidBody.AddForce(new Vector2(0, Jumpforce));
            }
        }
    }

    void StatusUpdate()
    {
       
    }

    void PhysicsUpdate()
    {
        if (Speed > MaxMoveSpeed) Speed = MaxMoveSpeed;
        if (Speed < -MaxMoveSpeed) Speed = -MaxMoveSpeed;

        this.transform.Translate(Vector3.right * Speed * Time.deltaTime);

        if (Speed <= Decelerate && Speed > 0) Speed = 0;
        else if (Speed > 0) Speed -= Decelerate;

        if (Speed >= -Decelerate && Speed < 0) Speed = 0;
        else if (Speed < 0) Speed += Decelerate;

        

    }

    void OnRailCheck()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down /** RaycastDistance*/;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, RaycastDistance, RailLayer);
        if (hit.collider != null)
        {
            OnRail = true;

            if (!OnRailPre)
            {
                if (Speed < 0)
                {
                    Speed = -OnRailSpeed;
                    OnRailDir = false;
                }
                else
                {
                    Speed = OnRailSpeed;
                    OnRailDir = true;
                }
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
            RigidBody.AddForce(new Vector2(0, Jumpforce));
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

