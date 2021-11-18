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


}
