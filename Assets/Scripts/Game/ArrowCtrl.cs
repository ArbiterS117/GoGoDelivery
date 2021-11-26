using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCtrl : MonoBehaviour
{
    public Transform Player;
    public Transform Target;
    //static public bool[] IsArrowed;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
       

       this.transform.position = Player.transform.position;


        Vector2 xx = new Vector2(0, 0);
        Vector2 up = new Vector2(0, 1);
        Vector3 PlayerPos = Player.transform.position;
        Vector3 TargetPos = Target.transform.position;

        xx = TargetPos - PlayerPos;

        var angle = Vector2.Angle(up, xx);

        Quaternion rot = GetComponent<Transform>().rotation;
        if (xx.x <= 0.0f) GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, angle));
        else GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, -angle));
    }
}
