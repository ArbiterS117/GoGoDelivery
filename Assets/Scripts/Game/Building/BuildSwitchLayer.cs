using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSwitchLayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            this.transform.Find("Hospital").gameObject.layer = 0;
            this.transform.Find("y_board").gameObject.layer = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            this.transform.Find("Hospital").gameObject.layer = 13;
            this.transform.Find("y_board").gameObject.layer = 13;
        }
    }
}
