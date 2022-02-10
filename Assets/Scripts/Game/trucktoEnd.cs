using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trucktoEnd : MonoBehaviour
{
    public bool isEnd = false;

    public GameObject BlackUI;


   
    // Update is called once per frame
    void Update()
    {
        if (isEnd)
        {
            BlackUI.GetComponent<Animator>().SetTrigger("END");
        }   
    }

    public void SetToEnd()
    {
        isEnd = true;
    }
}
