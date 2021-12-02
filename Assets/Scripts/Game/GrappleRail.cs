using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleRail : MonoBehaviour
{
    public float LifeTime = 0.0f;

    void Start()
    {
        LifeTime = 0.0f;
    }

    void Update()
    {
        LifeTime += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9) Debug.Log("111");

        //9 = rail
        if (other.gameObject.layer == 9)
        {
            if (other.GetComponent<GrappleRail>() == null) return;
            if (this.LifeTime >= other.GetComponent<GrappleRail>().LifeTime)
            {
                if (this.transform.parent != null) Destroy(this.transform.parent.gameObject);
                else Destroy(this.gameObject);
            }
        }
    }

}
