using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RailUICtrl : MonoBehaviour
{
    public GameObject player;
    public player playerScript;

    //public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.GrailEnergy == 1.0f)
        //GetComponent<Renderer>().enabled = false;
        {
            transform.Find("Background").gameObject.SetActive(false);
            transform.Find("Fill Area").transform.Find("Fill").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Background").gameObject.SetActive(true);
            transform.Find("Fill Area").transform.Find("Fill").gameObject.SetActive(true);
        }

        this.GetComponent<Slider>().value = (playerScript.GrailEnergy - playerScript.oriGrailEnergy) /
            (playerScript.MaxGrailEnergy - playerScript.oriGrailEnergy);
    }
}
