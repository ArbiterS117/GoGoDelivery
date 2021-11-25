using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    public Transform player;

    Vector3 oriPos;

    // Start is called before the first frame update
    void Start()
    {
        oriPos = player.position;
    }

    // Update is called once per frame
    void Update()
    {
        //player.position.x player.position.y, transform.position.z
        //transform.position = player.position;
        // Vector3 pos = player.position;
        // pos.z = transform.position.z;

        if (this.tag == "MainCamera") transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        //transform.position.Set(player.position.x, player.position.y, transform.position.z);
        //transform.position.x = 1.0f;

        else if (this.tag == "BGCamera")
        {
            Vector3 deltaDis = player.position - oriPos;
            deltaDis.y += 50.0f;
            deltaDis.x /= 3.0f;

            transform.position = oriPos + deltaDis;
        }
    }
}
