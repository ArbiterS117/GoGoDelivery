using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeUI : MonoBehaviour
{
    //private int totaltime1 = 5;
    public int totaltime2 = 0;
    private float intervaletime = 1;
    //public Text countdown1text;
    public Text countdown2text;

    // Start is called before the first frame update
    void Start()
    {
        //countdown1text.text = string.Format("{0:00}:{1:00}", (int)totaltime1 / 60, (float)totaltime1 % 60);
        countdown2text.text = string.Format("{0:00}:{1:00}", (int)totaltime2 / 60, (float)totaltime2 % 60);
        //StartCoroutine(CountDown());
    }

    // Update is called once per frame
    void Update()
    {
        //if (totaltime2 <= 0)
        //{
        //    GameObject player = GameObject.FindGameObjectWithTag("Player");
        //    ResultScore.SetFinalScore(player.GetComponent<player>().score);
        //    SceneManager.LoadScene("Result", LoadSceneMode.Single);

        //}

        int M = (int)(totaltime2 / 60);
        float S = totaltime2 % 60;

        intervaletime -= Time.deltaTime;
        if (intervaletime <= 0)
        {
            intervaletime += 1;
            totaltime2++;
            countdown2text.text = string.Format("{0:00}:{1:00}", M, S);
        }
        
        //if (totaltime2 <= 0)
        //{
        //    totaltime2 = 300;
        //}
    }
}
