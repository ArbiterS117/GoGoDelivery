using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulRailUI : MonoBehaviour
{
    public player playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();
    }

    // Update is called once per frame
    void Update()
    {
        int UsefulRail = playerScript.MaxGRailNum - playerScript.usedGrail;

        if (UsefulRail <= 0) transform.Find("1").gameObject.SetActive(false);
        else transform.Find("1").gameObject.SetActive(true);

        if (UsefulRail <= 1) transform.Find("2").gameObject.SetActive(false);
        else transform.Find("2").gameObject.SetActive(true);

        if (UsefulRail <= 2) transform.Find("3").gameObject.SetActive(false);
        else transform.Find("3").gameObject.SetActive(true);

        //if (UsefulRail <= 2) transform.Find("3").GetComponent<Renderer>().enabled = false;
        //else transform.Find("3").GetComponent<Renderer>().enabled = true;
    }
}
