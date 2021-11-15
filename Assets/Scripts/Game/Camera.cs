using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //player.position.x player.position.y, transform.position.z
        //transform.position = player.position;
        // Vector3 pos = player.position;
        // pos.z = transform.position.z;

        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        //transform.position.Set(player.position.x, player.position.y, transform.position.z);
        //transform.position.x = 1.0f;
    }
}
