using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSwitchLayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.transform.Find("Sprite").gameObject.layer = 13;
            other.transform.Find("SkateSprite ").gameObject.layer = 13;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.transform.Find("Sprite").gameObject.layer = 0;
            other.transform.Find("SkateSprite ").gameObject.layer = 0;
        }
    }
}
