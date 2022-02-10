using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    public Transform player;

    Vector3 oriPos;
    public Vector3 MouseDeltaPos;
    public float UpDis = 7.0f;

    // Start is called before the first frame update
    void Start()
    {
        oriPos = player.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //player.position.x player.position.y, transform.position.z
        //transform.position = player.position;
        // Vector3 pos = player.position;
        // pos.z = transform.position.z;

        if (player.GetComponent<player>().isDead) return;

        if (this.tag == "MainCamera")
        {
            transform.position = new Vector3(player.position.x, player.position.y + UpDis, transform.position.z);
            transform.position += MouseDeltaPos;

        }

        else if ( this.tag == "BGCamera")
        {
            MouseDeltaPos = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().MouseDeltaPos;
            transform.position = new Vector3(player.position.x, player.position.y + UpDis, transform.position.z);
            transform.position += MouseDeltaPos;

        }

        else if (this.tag == "BGSpriteCamera")
        {
            Vector3 deltaDis = player.position - oriPos;
            deltaDis.y += 50.0f;
            deltaDis.x /= 3.0f;

            transform.position = oriPos + deltaDis;
        }
    }
}
