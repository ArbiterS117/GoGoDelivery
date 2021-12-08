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
    public int   playerDir = 1;   // 右
    Vector3 oriScale;
    public float MaxMoveSpeed = 10.0f;
    public float oriMaxMoveSpeed = 10.0f;
    public float Decelerate = 15.0f;

    public bool OnRail = false;
    public bool OnRailPre = false;

    public float OnRailSpeed = 30.0f;
    public bool OnRailDir = true;// 1: Right 0: Left

    public float OnRailUpOffset = 2.0f;

    public float MaxOnRailSpeed = 40.0f;
    public float MinOnRailSpeed = 20.0f;

    public Vector2 RailNormal　= new Vector2(0.0f, 1.0f);
    public float ExitRailJumpForce = 25.0f;
    [Range(40f, 100.0f)]
    public float ExitCurvedRailJumpForce = 50.0f;
    public bool OnRailJumpTrigger = false;

    //=========================== game
    public int score = 0;
    //public bool HoldingNimotu = false;

    public int HoldingNimotsuNum = 0;
    static public int MaxNimotsuNum = 3;

    public Transform[] StartRebornPoint;
    public bool[] StartRebornPointUsed;
    public Transform[] CRebornPoint;
    public bool[] CRebornPointUsed;
    public int CRebornPointNum;

    bool isCargoAreaArrow = false;
    List<GameObject> ArrowList = new List<GameObject>();
    List<GameObject> StartArrowList = new List<GameObject>();

    float oriGravity = 0.0f;

    //=========================== grapple rail
    static public int MaxGRailNum = 3;
    public int usedGrail = 0;
    public List<GameObject> GRail = new List<GameObject>();
    public float GrailEnergy = 1.0f;    // defult : 1.0
    public float oriGrailEnergy = 1.0f; // defult : 1.0
    [Range(1.0f, 3.0f)]
    public float MaxGrailEnergy = 2.0f;    // defult : 2.0
    [Range(0f, 2.0f)]
    public float EnergyStore = 0.01f;  // defult : 1.0
    public bool  CanStoreEnergy = true;
    public float CaculatedJumpForce = 1.0f;
    public bool  IsShooting = false;
    public float ShootingGravity = 0.5f;
    public bool  ShootingJump = false;
    public float ShootingJumpVelocity = 5.0f;
    public float ShootingAddSpeed = 0.1f;
    public float ShootingDecelerate = 30.0f;

    //=========================== Useful Rail Charge Timer
    public float RailChargeTime = 1;
    public float Rail_CTimer = 0;

    void Start()
    {
        RigidBody = this.GetComponent<Rigidbody2D>();
        Speed = 0.0f;
      
        oriGravity = RigidBody.gravityScale;
        CRebornPointNum = CRebornPointUsed.Length;

        oriScale = this.transform.Find("Sprite").localScale;
        oriGrailEnergy = GrailEnergy;
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
        if (OnRail)
        {
            RigidBody.gravityScale = 0.0f;
        }
        else if (IsShooting)
        {
            RigidBody.gravityScale = ShootingGravity;
            //MaxMoveSpeed = ShootingMAXSpeed;
        }
        else
        {
            RigidBody.gravityScale = oriGravity;
            MaxMoveSpeed = oriMaxMoveSpeed;
        }

        ///============================================================
        if (!OnRail)
            InputUpdate();
        else
            OnRailInputUpdate();

        StatusUpdate();

        playerSub.AnimationUpdate(this, GetComponent<Animator>());
        playerSub.GrappleUpdate(this);
        playerSub.ParticleUpdate(this);
       
    }

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down * RaycastDistance;

        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, RaycastDistance, GroundLayer);
        if (hit.collider != null)
        {
            if (IsGround && !OnRail)
            {
                //pos & rot
                Vector3 pos = transform.position;
                Vector2 UpOffset = hit.normal * OnRailUpOffset;
                //this.transform.position = new Vector3(hit.point.x, hit.point.y + OnRailUpOffset, pos.z);

                Quaternion rot = GetComponent<Transform>().rotation;
                Vector2 up = new Vector2(0, 1);
                var angle = Vector2.Angle(up, hit.normal);
                if (hit.normal.x <= 0.0f) this.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, angle));
                else this.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, -angle));
            }
            return true;
        }

        return false;
    }

    void InputUpdate()
    {
        if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow)) //左
        {
            if(IsShooting)this.Speed -= ShootingAddSpeed;
            else          this.Speed -= AddSpeed;           
        }
        if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) //右
        {
            if(IsShooting)this.Speed += ShootingAddSpeed;
            else          this.Speed += AddSpeed;

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
            Rail_CTimer += Time.deltaTime;
            if(Rail_CTimer >= RailChargeTime)
            {
                usedGrail--;
                if (usedGrail < 0) usedGrail = 0;
                Rail_CTimer = 0;
            }

            //usedGrail = 0;
        }

        if (OnRail)
        {
            this.GetComponent<Collider2D>().isTrigger = true;
        }
        else
        {
            this.GetComponent<Collider2D>().isTrigger = false;
        }

        if (Speed > 0.0f) playerDir = 1;
        else if (Speed < 0.0f) playerDir = -1;
        this.transform.Find("Sprite").localScale= new Vector3(oriScale.x * playerDir, oriScale.y, oriScale.z);

        if(HoldingNimotsuNum == 0)
        {
            if(isCargoAreaArrow == false)
            {
                for (int i = 0; i < StartRebornPoint.Length; i++)
                {
                    GameObject arrow = (GameObject)Resources.Load("StartArrow");
                    GameObject targetArrow = (GameObject)Instantiate(arrow,
                                                  this.transform.position,
                                                   Quaternion.identity);
                    targetArrow.GetComponent<ArrowCtrl>().Player = this.transform;
                    targetArrow.GetComponent<ArrowCtrl>().Target = StartRebornPoint[i];
                    StartArrowList.Add(targetArrow);
                }
                isCargoAreaArrow = true;

            }
        }
        else
        {
            for (int i = 0; i < StartArrowList.Count; i++)
            {
                GameObject temp = StartArrowList[0];
                StartArrowList.Remove(temp);
                Destroy(temp);
            }
            isCargoAreaArrow = false;
        }

    }

    void PhysicsUpdate()
    {
        if (Speed > MaxMoveSpeed) Speed = MaxMoveSpeed;
        if (Speed < -MaxMoveSpeed) Speed = -MaxMoveSpeed;

        this.transform.Translate(Vector3.right * Speed * Time.deltaTime);

        if (IsShooting)
        {
            if (Speed <= ShootingDecelerate * Time.deltaTime && Speed > 0) Speed = 0;
            else if (Speed > 0) Speed -= ShootingDecelerate * Time.deltaTime;

            if (Speed >= -ShootingDecelerate * Time.deltaTime && Speed < 0) Speed = 0;
            else if (Speed < 0) Speed += ShootingDecelerate * Time.deltaTime;
        }
        else
        {
            if (Speed <= Decelerate * Time.deltaTime && Speed > 0) Speed = 0;
            else if (Speed > 0) Speed -= Decelerate * Time.deltaTime;

            if (Speed >= -Decelerate * Time.deltaTime && Speed < 0) Speed = 0;
            else if (Speed < 0) Speed += Decelerate * Time.deltaTime;
        }

        //Exit Rail
        if (OnRailPre)
        {
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.0f);
            //jump
            if (((Speed >= 0.0f && RailNormal.x < -0.7f) || (Speed < 0.0f && RailNormal.x > 0.7f) ) && !OnRailJumpTrigger)
            {
                CaculatedJumpForce = Mathf.Abs(RailNormal.x) * ExitCurvedRailJumpForce * 2 - ExitCurvedRailJumpForce;
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, CaculatedJumpForce);
            }
            else if (((Speed >= 0.0f && RailNormal.x < -0.2f) || (Speed < 0.0f && RailNormal.x > 0.2f)) && !OnRailJumpTrigger)
            {
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, ExitRailJumpForce);
            }
        }

    }

    void OnRailCheck()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down /** RaycastDistance*/;
        this.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

        RaycastHit2D hit = Physics2D.Raycast(position, direction, RaycastDistance, RailLayer);
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.layer == 12) // 12: UnPanGround
            {
                IsGround = true;
                OnRail = false;
                return;
            }
       

            if (RigidBody.velocity.y <= 0.0f)
            {
                OnRail = true;
                if(playerDir == 1) OnRailDir = true;
                if(playerDir ==-1) OnRailDir = false;
                OnRailJumpTrigger = false;
            }

            if (OnRail == true)
            {               
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

      
    }

    //  当たり判定
    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other.tag == "CargoArea")
        {
            int targetNeed = 0;
            if (HoldingNimotsuNum < MaxNimotsuNum)
            {
                targetNeed = MaxNimotsuNum - HoldingNimotsuNum;
                HoldingNimotsuNum = MaxNimotsuNum;
            }
            //お客様再生
            for(int i = 0; i < targetNeed; i++)
            {
                while (true)
                {
                    int index = (int)Random.Range(0, CRebornPointNum - 0.001f);
                    if (CRebornPointUsed[index] == false)
                    {
                        GameObject customer = (GameObject)Resources.Load("customer");
                        GameObject target = (GameObject)Instantiate(customer,
                                                      CRebornPoint[index].position,
                                                      Quaternion.identity);
                        CRebornPointUsed[index] = true;
                        target.GetComponent<Custormer>().index = index;

                        //矢印再生
                        GameObject arrow = (GameObject)Resources.Load("arrow");
                        GameObject targetArrow = (GameObject)Instantiate(arrow,
                                                      this.transform.position,
                                                       Quaternion.identity);
                        targetArrow.GetComponent<ArrowCtrl>().Player = this.transform;                      
                        targetArrow.GetComponent<ArrowCtrl>().Target = CRebornPoint[index];
                        ArrowList.Add(targetArrow);
                        target.GetComponent<Custormer>().Arrow = targetArrow;
                        break;
                    }
                }

            }
        }

        if(other.tag == "customer")
        {
            if (HoldingNimotsuNum > 0)
            {              
                HoldingNimotsuNum--;
                CRebornPointUsed[other.GetComponent<Custormer>().index] = false;

                GameObject temp = other.GetComponent<Custormer>().Arrow;
                ArrowList.Remove(temp);
                Destroy(temp);
                Destroy(other.gameObject);                                 

                score++;                                  
            }
        }
    }

}

