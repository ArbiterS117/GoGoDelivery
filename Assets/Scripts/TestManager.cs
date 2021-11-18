using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    //public Transform[] RebornPoint;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = (GameObject)Resources.Load("Nimotu");

        // プレハブを元にオブジェクトを生成する
        GameObject item1 = (GameObject)Instantiate(obj,
                                                      new Vector3(15.0f, 0.0f, 11.0f),
                                                      Quaternion.identity);
        GameObject item2 = (GameObject)Instantiate(obj,
                                              new Vector3(5.0f, 10.0f, 11.0f),
                                              Quaternion.identity);
        GameObject item3 = (GameObject)Instantiate(obj,
                                              new Vector3(-15.0f, 0.0f, 11.0f),
                                              Quaternion.identity);

        // Goal生成
        GameObject xxx = (GameObject)Resources.Load("customer");
        GameObject Goal1 = (GameObject)Instantiate(xxx,
                                      new Vector3(20.0f, 0.0f, 11.0f),
                                      Quaternion.identity);
        GameObject Goal2 = (GameObject)Instantiate(xxx,
                              new Vector3(-10f, 0.0f, 11.0f),
                              Quaternion.identity);
        GameObject Goal3 = (GameObject)Instantiate(xxx,
                                      new Vector3(5.0f, 0.0f, 11.0f),
                                      Quaternion.identity);

        // 連結
        item1.GetComponent<Nimotu>().GGGoal = Goal1;
        item2.GetComponent<Nimotu>().GGGoal = Goal2;
        item3.GetComponent<Nimotu>().GGGoal = Goal3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
