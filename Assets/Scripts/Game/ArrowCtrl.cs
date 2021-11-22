using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCtrl : MonoBehaviour
{
    public GameObject playerOBJ;
    public player playerScript;
    public GameObject Nimotsu;
    //static public bool[] IsArrowed;

    // Start is called before the first frame update
    void Start()
    {
        playerOBJ = GameObject.FindGameObjectWithTag("Player");
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();
        //for (int i = 0; i < player.MaxNimotsuNum; i++)
        //    IsArrowed[i] = false;
        //Nimotsu = null;
    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < player.MaxNimotsuNum; i++)
        //{
        //    if (!IsArrowed[i]) 
        //}

        GetComponent<Transform>().position = playerOBJ.GetComponent<Transform>().position;

        //for (int i = 0; i < player.MaxNimotsuNum; i++)
        //{
        //    if (playerScript.NimotsuHolded[i] && IsArrowed[i] == false)
        //    {
        //        Nimotsu = playerScript.NimotsuHolded[i];
        //        IsArrowed[i] = true;
        //        break;
        //    }
        //}

        Nimotsu = playerOBJ.GetComponent<player>().NimotsuHolded[0];
        if (Nimotsu == null) GetComponent<Renderer>().enabled = false;

        else
        {
            GetComponent<Renderer>().enabled = true;

            Vector2 xx = new Vector2(0, 0);
            Vector2 up = new Vector2(0, 1);
            Vector3 playerPos = playerOBJ.GetComponent<Transform>().position;
            Vector3 NimotsuPos = Nimotsu.GetComponent<Nimotu>().GGGoal.GetComponent<Transform>().position;

            xx = NimotsuPos - playerPos;

            var angle = Vector2.Angle(up, xx);

            Quaternion rot = GetComponent<Transform>().rotation;
            if(xx.x <= 0.0f) GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, angle));
            else GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, -angle));

            Debug.Log(rot);
        }
    }
}
