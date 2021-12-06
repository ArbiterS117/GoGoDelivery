using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerSub : MonoBehaviour
{
    public static float Idle2Time  = 7.0f;
           static float Idle2CTime = 0.0f;


  public static void AnimationUpdate(player player, Animator animator)
    {
        //Idle2 
        if (player.Speed == 0.0f && player.RigidBody.velocity.y == 0.0f)
        {
            Idle2CTime += Time.deltaTime;
            if (Idle2CTime >= Idle2Time)
            {
                animator.SetTrigger("Idle2");
                Idle2CTime = 0.0f;
            }
        }
        else
        {
            Idle2CTime = 0.0f;
        }

        
        animator.SetFloat("SpeedX", Mathf.Abs(player.Speed));
        animator.SetFloat("SpeedY", player.RigidBody.velocity.y);

        if (player.IsGround) animator.SetBool("Ground",true);
        else animator.SetBool("Ground", false);
    }

    public static void GrappleUpdate(player player)
    {
        if (player.usedGrail >= player.MaxGRailNum) return;

        if(Input.GetMouseButton(0))
        {
            if (player.CanStoreEnergy == true)
            {
                player.IsShooting = true;
                player.GrailEnergy += player.EnergyStore * Time.deltaTime;
                if (player.GrailEnergy >= player.MaxGrailEnergy)
                {
                    player.CanStoreEnergy = false;
                    MouseUp(player);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (player.CanStoreEnergy == true)
            {
                MouseUp(player);
            }
            player.CanStoreEnergy = true;
        }


    }

    static GameObject ShootGrapple(player player)
    {
        TargetAim Aim = GameObject.FindGameObjectWithTag("Aim").GetComponent<TargetAim>();
        int dir = 0;
        if (Aim.AimPos.x >= 0.0f) { player.OnRailDir = true; dir = 1; }
        else { player.OnRailDir = false; dir = -1; }

        Vector2 AimPos = new Vector2(Aim.AimPos.x, Aim.AimPos.y);
        Vector2 right = new Vector2(1, 0);
        var angle = Vector2.Angle(right, AimPos);

        if (angle >= 30.0f && angle <= 150.0f && AimPos.y > 0.0f) // Verticle
        {
            GameObject obj = (GameObject)Resources.Load("CurveRail");
            GameObject Item = (GameObject)Instantiate(obj,
                                                          player.transform.position,
                                                          Quaternion.identity);
            // Angle & position currection      
            Vector3 ItemPosOri = Item.transform.position;
            Item.transform.position = new Vector3(ItemPosOri.x, ItemPosOri.y - 2.0f, ItemPosOri.z);
            EdgeCollider2D EdgeCollider = Item.GetComponent<EdgeCollider2D>();
            float vx = 0.0f, vy = 0.0f;
            float radius = 15.0f / player.GrailEnergy;
            List<Vector2> newPoint = new List<Vector2>();
            for (int i = 0; i < EdgeCollider.pointCount; i++) {
                float perSinAngle = Mathf.Sin((Mathf.PI * 0.25f * player.GrailEnergy) / (EdgeCollider.pointCount - 2) * i);
                float perCosAngle = Mathf.Cos((Mathf.PI * 0.25f * player.GrailEnergy) / (EdgeCollider.pointCount - 2) * i);
                     if (i == 0) { vx = -2.5f * dir; vy = -2.0f; newPoint.Add(new Vector2(vx, vy)); }
                else if (i == 1) { vx =  2.5f * dir; vy = -2.0f; newPoint.Add(new Vector2(vx, vy)); }
                else { vx = 2.5f * dir + radius * perSinAngle * dir; vy = -2.0f + radius - (radius * perCosAngle); newPoint.Add(new Vector2(vx, vy)); }

            }
            Item.GetComponent<EdgeCollider2D>().SetPoints(newPoint);

            //base scaling
            float baseDis = Vector2.Distance(EdgeCollider.points[0], EdgeCollider.points[1]);
            for (int i = 1; i < EdgeCollider.pointCount; i++)
            {
                // Generate sprite
                GameObject spriteobj = (GameObject)Resources.Load("CurveRailSprite");
                Vector2 deltaPos = (EdgeCollider.points[i] - EdgeCollider.points[i - 1]) / 2;
                Vector2 ItemPos = Item.transform.position;
                Vector2 ssPos = ItemPos + EdgeCollider.points[i - 1] + deltaPos;
                Vector3 sPos = new Vector3(ssPos.x, ssPos.y, Item.transform.position.z);
                GameObject sprite = (GameObject)Instantiate(spriteobj,
                                                                sPos,
                                                                Quaternion.identity);
                sprite.transform.SetParent(Item.transform);
                //angle
                float sAngle = Vector2.Angle(right, EdgeCollider.points[i] - EdgeCollider.points[i - 1]);
                Quaternion rot = sprite.transform.rotation;
                sprite.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, sAngle));
                //scale
                float pointDis = Vector2.Distance(EdgeCollider.points[i], EdgeCollider.points[i-1]);
                float newScale = pointDis / baseDis;
                Vector3 oriScale = sprite.transform.localScale;
                sprite.transform.localScale = new Vector3(oriScale.x * newScale, oriScale.y, oriScale.z);

            }
            return Item;


        }
        else // normal
        {
            GameObject obj = (GameObject)Resources.Load("GrappleRail");
            GameObject Item = (GameObject)Instantiate(obj,
                                                          player.transform.position,
                                                          Quaternion.identity);
            // Angle & position currection
            Quaternion rot = Item.transform.rotation;
            if (AimPos.y <= 0.0f) Item.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, -angle));
            else Item.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, angle));
            Vector3 ItemPosOri = Item.transform.position;
            Item.transform.position = new Vector3(ItemPosOri.x, ItemPosOri.y - 4.0f, ItemPosOri.z);

            //Scaleing
            Vector3 scl = Item.transform.localScale;
            Item.transform.localScale = new Vector3(scl.x * player.GrailEnergy, scl.y, scl.z);

            return Item;


        }


    }

    static void MouseUp(player player)
    {
        if (player.GRail.Count < player.MaxGRailNum)
        {
            player.GRail.Add(ShootGrapple(player));
            player.usedGrail += 1;
        }
        else
        {
            GameObject temp = player.GRail[0];
            player.GRail.Remove(player.GRail[0]);
            GameObject.Destroy(temp);
            player.GRail.Add(ShootGrapple(player));
            player.usedGrail += 1;
        }
        player.IsShooting = false;
        player.GrailEnergy = player.oriGrailEnergy;

    }


}
