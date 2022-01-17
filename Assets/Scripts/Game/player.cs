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

   // bool isCargoAreaArrow = false;
   // List<GameObject> ArrowList = new List<GameObject>();
   // List<GameObject> StartArrowList = new List<GameObject>();

    float oriGravity = 0.0f;

    //=========================== grapple rail
    [Header("grapple rail")]
    public int MaxGRailNum = 1;
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

    //=========================== Boost
    [Header("Boost")]
    public bool IsBoosting = false;
    public float BoostMaxSpeed = 60.0f;
    public float BoostTime = 1.5f;
    public float BoostCTime = 0.0f;
    public GameObject BoostEffect;

    //=========================== Dashing
    public float DashEffectOutTime =  0.3f;
    public float DashEffectOutCTime = 0.0f;
    public GameObject DashEffect;

    //=========================== WallJump
    public float WallJumpSpeed = 30.0f;
    public float WallJumpForce = 5.0f;
    public GameObject WallJumpEffect;

    //=========================== Useful Rail Charge Timer
    public float RailChargeTime = 1;
    public float Rail_CTimer = 0;

    //=========================== WallJump
    public float FlyPlatJumpForce = 2200.0f;
    public GameObject FlyPlatEffect;

    //=========================== Event
    [Header("Event")]
    public bool IsJamming = false;
    public float JammingMaxSpeed = 20.0f;

    //=========================== Effect Prefab
    [Header("Effect Prefab")]
    public GameObject CargoEffect;
    public GameObject CustormerEffect;

    //=========================== Debug
    public Transform[] savepoint;
    public int CurSavePoint;
    public GameObject eventtruck;

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
        this.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));

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
        if (RigidBody.velocity.y > 0.0f) return false;

        Vector2 position = transform.position;
        Vector2 direction = Vector2.down * RaycastDistance;

        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, RaycastDistance, GroundLayer);
        if (hit.collider != null)
        {
            if (IsGround && !OnRail)
            {
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.0f);

                //pos & rot
                Vector3 pos = transform.position;
                Vector2 UpOffset = hit.normal * OnRailUpOffset;
                this.transform.position = new Vector3(hit.point.x, hit.point.y + OnRailUpOffset, pos.z);

                Quaternion rot = GetComponent<Transform>().rotation;
                Vector2 up = new Vector2(0, 1);
                var angle = Vector2.Angle(up, hit.normal);
                if (hit.normal.x <= 0.0f) this.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, angle));
                else this.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, -angle));

                
            }
            if (OnRail) OnRail = false;

            return true;
        }

        return false;
    }

    void InputUpdate()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) //左
        {
            if(IsShooting)this.Speed -= ShootingAddSpeed;
            else          this.Speed -= AddSpeed;
            GetComponent<Animator>().SetBool("isPressingMove", true);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) //右
        {
            if(IsShooting)this.Speed += ShootingAddSpeed;
            else          this.Speed += AddSpeed;
            GetComponent<Animator>().SetBool("isPressingMove", true);
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            GetComponent<Animator>().SetBool("isPressingMove", false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            IsBoosting = true;
            Speed = BoostMaxSpeed * playerDir;
            //エフェクト
            GameObject effect = Instantiate(BoostEffect, this.transform.position, Quaternion.Euler(new Vector3(0.0f, 90.0f * playerDir, 0.0f))) as GameObject;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGround) 
            {
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.1f);
                RigidBody.AddForce(new Vector2(0, Jumpforce));
            }
           
        }

        //================
        if (Input.GetKeyDown(KeyCode.F1))
        {
            transform.position = savepoint[CurSavePoint].position;
            eventtruck.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            CurSavePoint++;
            if (CurSavePoint >= savepoint.Length) CurSavePoint = 0;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            MaxGRailNum += 1;

        }
    }

    void StatusUpdate()
    {
        if (IsGround)
        {
            RigidBody.gravityScale = 0.0f;

            Rail_CTimer += Time.deltaTime;
            if(Rail_CTimer >= RailChargeTime)
            {
                usedGrail--;
                if (usedGrail < 0) usedGrail = 0;
                Rail_CTimer = 0;
            }

        }
      

      
        if (Speed > 0.0f) playerDir = 1;
        else if (Speed < 0.0f) playerDir = -1;
        this.transform.Find("Sprite").localScale= new Vector3(oriScale.x * playerDir, oriScale.y, oriScale.z);

        //savepoint
        for (int i = 0; i < savepoint.Length; i++)
        {
            if (transform.position.x >= savepoint[i].position.x) CurSavePoint = i;
        }




       // if(HoldingNimotsuNum == 0)
       // {
       //     if(isCargoAreaArrow == false)
       //     {
       //         for (int i = 0; i < StartRebornPoint.Length; i++)
       //         {
       //             GameObject arrow = (GameObject)Resources.Load("StartArrow");
       //             GameObject targetArrow = (GameObject)Instantiate(arrow,
       //                                           this.transform.position,
       //                                            Quaternion.identity);
       //             targetArrow.GetComponent<ArrowCtrl>().Player = this.transform;
       //             targetArrow.GetComponent<ArrowCtrl>().Target = StartRebornPoint[i];
       //             StartArrowList.Add(targetArrow);
       //         }
       //         isCargoAreaArrow = true;
       //
       //     }
       // }
       // else
       // {
       //     for (int i = 0; i < StartArrowList.Count; i++)
       //     {
       //         GameObject temp = StartArrowList[0];
       //         StartArrowList.Remove(temp);
       //         Destroy(temp);
       //     }
       //     isCargoAreaArrow = false;
       // }

        //==============Boosting
        if (IsBoosting)
        {
            GetComponent<Animator>().SetBool("isDashing", true);
            MaxMoveSpeed = BoostMaxSpeed;
            BoostCTime += Time.deltaTime;
            if(BoostCTime >= BoostTime)
            {
                BoostCTime = 0.0f;
                IsBoosting = false;
                MaxMoveSpeed = oriMaxMoveSpeed;
            }

          
            //==Dash Effect
            DashEffectOutCTime += Time.deltaTime;
            if(DashEffectOutCTime >= DashEffectOutTime)
            {
                //エフェクト
                GameObject effect = Instantiate(DashEffect, this.transform.position, Quaternion.Euler(new Vector3(0.0f, -90.0f * playerDir, 0.0f))) as GameObject;
                DashEffectOutCTime = 0.0f;
            }
        }
        else
        {
            GetComponent<Animator>().SetBool("isDashing", false);
        }

        //==============Jamming
        if (IsJamming)
        {
            MaxMoveSpeed = JammingMaxSpeed;
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
            //RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.0f);

            //jump
            if (((Speed >= 0.0f && RailNormal.x < -0.7f) || (Speed < 0.0f && RailNormal.x > 0.7f) ) && !OnRailJumpTrigger)
            {
                //CaculatedJumpForce = Mathf.Abs(RailNormal.x) * ExitCurvedRailJumpForce * 2 - ExitCurvedRailJumpForce;
                //RigidBody.velocity = new Vector2(RigidBody.velocity.x, CaculatedJumpForce);
            }
            else if (((Speed >= 0.0f && RailNormal.x < -0.2f) || (Speed < 0.0f && RailNormal.x > 0.2f)) && !OnRailJumpTrigger)
            {
                //RigidBody.velocity = new Vector2(RigidBody.velocity.x, ExitRailJumpForce);
            }
        }

    }

    void OnRailCheck()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down /** RaycastDistance*/;
        //this.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        
        RaycastHit2D hit = Physics2D.Raycast(position, direction, RaycastDistance, RailLayer);
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.layer == 12) // 12: UnPanGround
            {
                if (RigidBody.velocity.y <= 0.0f)
                {
                    IsGround = true;
                    OnRail = false;
                    return;
                }
                OnRail = false;
                GetComponent<Animator>().SetBool("OnRail", false);
            }


            if (RigidBody.velocity.y <= 0.0f)
            {
                OnRail = true;
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.1f);
                if (playerDir == 1) OnRailDir = true;
                if (playerDir == -1) OnRailDir = false;
            }

            if (OnRail == true)
            {
                RailNormal = hit.normal;
                //pos & rot
                Vector3 pos = transform.position;
                Vector2 UpOffset = hit.normal * OnRailUpOffset;
                this.transform.position = new Vector3(hit.point.x, hit.point.y + OnRailUpOffset, pos.z);

                Quaternion rot = GetComponent<Transform>().rotation;
                Vector2 up = new Vector2(0, 1);
                var angle = Vector2.Angle(up, hit.normal);
                if (hit.normal.x <= 0.0f) this.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, angle));
                else this.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, -angle));

                GetComponent<Animator>().SetBool("OnRail", true);
                GetComponent<Animator>().SetBool("Fireing", false);
                GetComponent<Animator>().SetBool("IsShooting", false);
            }


        }
        else
        {
            OnRail = false;
            GetComponent<Animator>().SetBool("OnRail", false);
        }
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
        }

        if (Input.GetMouseButtonDown(1))
        {
            IsBoosting = true;
            Speed = BoostMaxSpeed * playerDir;
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

    //===================Anim
    void setAnimFireingF()
    {
        GetComponent<Animator>().SetBool("Fireing", false);

    }

    //  当たり判定
    private void OnTriggerEnter2D(Collider2D other)
    {

       //if(other.tag == "CargoArea")
       //{
       //    int targetNeed = 0;
       //    if (HoldingNimotsuNum < MaxNimotsuNum)
       //    {
       //        targetNeed = MaxNimotsuNum - HoldingNimotsuNum;
       //        HoldingNimotsuNum = MaxNimotsuNum;
       //    }
       //    //お客様再生
       //    for(int i = 0; i < targetNeed; i++)
       //    {
       //        while (true)
       //        {
       //            int index = (int)Random.Range(0, CRebornPointNum - 0.001f);
       //            if (CRebornPointUsed[index] == false)
       //            {
       //                GameObject customer = (GameObject)Resources.Load("customer");
       //                GameObject target = (GameObject)Instantiate(customer,
       //                                              CRebornPoint[index].position,
       //                                              Quaternion.identity);
       //                CRebornPointUsed[index] = true;
       //                target.GetComponent<Custormer>().index = index;
       //
       //                //矢印再生
       //                GameObject arrow = (GameObject)Resources.Load("arrow");
       //                GameObject targetArrow = (GameObject)Instantiate(arrow,
       //                                              this.transform.position,
       //                                               Quaternion.identity);
       //                targetArrow.GetComponent<ArrowCtrl>().Player = this.transform;                      
       //                targetArrow.GetComponent<ArrowCtrl>().Target = CRebornPoint[index];
       //                ArrowList.Add(targetArrow);
       //                target.GetComponent<Custormer>().Arrow = targetArrow;
       //
       //                //エフェクト
       //                GameObject effect = Instantiate(CargoEffect, this.transform.position, Quaternion.identity) as GameObject;
       //
       //                break;
       //            }
       //        }
       //
       //    }
       //}

       //if(other.tag == "customer")
       //{
       //    if (HoldingNimotsuNum > 0)
       //    {              
       //        HoldingNimotsuNum--;
       //        CRebornPointUsed[other.GetComponent<Custormer>().index] = false;
       //
       //        GameObject temp = other.GetComponent<Custormer>().Arrow;
       //        ArrowList.Remove(temp);
       //        Destroy(temp);
       //        Destroy(other.gameObject);                                 
       //
       //        score++;
       //
       //        //エフェクト
       //        GameObject effect = Instantiate(CustormerEffect, this.transform.position, Quaternion.identity) as GameObject;
       //    }
       //}

        if (other.transform.tag == "JamZone")
        {
            this.IsJamming = true;
            this.transform.Find("speedDownIcon").gameObject.SetActive(true);
        }

        if(other.transform.tag == "RailJumpTrigger")
        {
            float jumpForce = other.GetComponent<RailJumpData>().JumpForce;
            Speed = BoostMaxSpeed * playerDir;
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.1f);
            RigidBody.AddForce(new Vector2(0, jumpForce));
            //エフェクト
            GameObject effect = Instantiate(FlyPlatEffect, this.transform.position, Quaternion.identity) as GameObject;

            if (other.GetComponent<RailJumpData>().DestroyAfterUsed) Destroy(other.transform.parent.gameObject);
        }

        if (other.transform.tag == "CurveRailJumpTrigger")
        {
            float jumpForce = other.GetComponent<RailJumpData>().JumpForce;
            Speed =  0.0f;
            RigidBody.velocity = new Vector2(0.0f, 0.1f);
            RigidBody.AddForce(new Vector2(0, jumpForce));
            //エフェクト
            GameObject effect = Instantiate(FlyPlatEffect, this.transform.position, Quaternion.identity) as GameObject;

            if (other.GetComponent<RailJumpData>().DestroyAfterUsed) Destroy(other.transform.parent.gameObject);
        }
        

        if (other.transform.tag == "RailGunItem")
        {
            MaxGRailNum += 1;
            Destroy(other.gameObject);
        }

        if (other.transform.tag == "Damage")
        {
            transform.position = savepoint[CurSavePoint].position;
            eventtruck.SetActive(false);
        }

        if (other.transform.tag == "ATKEnemyZone")
        {
            RailJumpData TargetData = other.GetComponent<RailJumpData>();
            float jumpForce = TargetData.JumpForce;
            Speed = BoostMaxSpeed * playerDir;
            RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.1f);
            RigidBody.AddForce(new Vector2(0, jumpForce));
            //エフェクト
            GameObject effect = Instantiate(FlyPlatEffect, this.transform.position, Quaternion.identity) as GameObject;

            if (TargetData.DestroyAfterUsed) Destroy(other.transform.parent.gameObject);

            TargetData.HP -= 1;
            if (TargetData.HP <= 0){
                other.GetComponentInParent<Animator>().SetTrigger("Destroyed");
                other.transform.parent.Find("explode").gameObject.SetActive(true);
                other.transform.parent.Find("DamageCollider").gameObject.SetActive(false);
            }
        }

        if (other.transform.tag == "coin")
        {
            score += 1;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "FlyTrigger")
        {
            if (IsBoosting)
            {
                Speed = BoostMaxSpeed * playerDir;
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.1f);
                RigidBody.AddForce(new Vector2(0, FlyPlatJumpForce));
                //エフェクト
                GameObject effect = Instantiate(FlyPlatEffect, this.transform.position, Quaternion.identity) as GameObject;
            }
        }

        if (other.transform.tag == "JamZone")
        {
            this.IsJamming = false;
            this.transform.Find("speedDownIcon").gameObject.SetActive(false);
        }
    }

    //======Collision
    void OnCollisionStay2D(Collision2D other)
    {
        if(other.transform.tag == "Wall")
        {
           
            if (IsBoosting && Mathf.Abs(Speed) >= WallJumpSpeed || OnRail)
            {
                Speed *= -1 * 0.75f;
                RigidBody.velocity = new Vector2(RigidBody.velocity.x, 0.1f);
                RigidBody.AddForce(new Vector2(0, WallJumpForce));
                //エフェクト
                GameObject effect = Instantiate(WallJumpEffect, this.transform.position, Quaternion.identity) as GameObject;
                OnRail = false;
            }
            else
            {
                Speed = 0.0f;
            }
        }

       
    }

   

}


