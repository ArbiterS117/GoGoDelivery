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
        if (Input.GetMouseButtonDown(0))
        {
            ShootGrapple(player);
        }


    }

    static void ShootGrapple(player player)
    {
        TargetAim Aim = GameObject.FindGameObjectWithTag("Aim").GetComponent<TargetAim>();

        GameObject obj = (GameObject)Resources.Load("GrappleRail");
        GameObject Item = (GameObject)Instantiate(obj,
                                                      player.transform.position,
                                                      Quaternion.identity);

        // Angle & position currection
        Vector2 xx = new Vector2(Aim.AimPos.x, Aim.AimPos.y);
        Vector2 up = new Vector2(1, 0);
        var angle = Vector2.Angle(up, xx);
        Quaternion rot = Item.transform.rotation;
        if (xx.y <= 0.0f) Item.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, -angle));
        else Item.transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, angle));
        Vector3 ItemPosOri = Item.transform.position;
        Item.transform.position = new Vector3(ItemPosOri.x, ItemPosOri.y - 4.0f, ItemPosOri.z);
    }


}
