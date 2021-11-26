using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAim : MonoBehaviour
{
    public GameObject player;
    public Camera cam;
    public Vector3 AimPos;

    [Range(0.0f, 0.05f)]
    public float CameraMovingRate = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        AimPos = Input.mousePosition;
        this.transform.localPosition = new Vector2( AimPos.x - (Screen.width / 2), AimPos.y - (Screen.height / 2));
        AimPos.x -= (Screen.width / 2);
        AimPos.y -= (Screen.height / 2);

        cam.MouseDeltaPos.x = 0.0f;
        cam.MouseDeltaPos.y = 0.0f;
        cam.MouseDeltaPos.x += AimPos.x * CameraMovingRate;
        cam.MouseDeltaPos.y += AimPos.y * CameraMovingRate;

       

    }
}
