using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultCtrl : MonoBehaviour
{
    public Text ScoreText;
    public Text DeathText;
    public Text ClearTimeText;

    // Start is called before the first frame update
    void Start()
    {
        ScoreText.text     = ResultScore.GetFinalScore().ToString();
        DeathText.text     = ResultScore.GetFinalDeathTime().ToString();
        
        ClearTimeText.text = string.Format("{0:00}:{1:00}", (int)ResultScore.GetFinalClearTime() / 60, (float)ResultScore.GetFinalClearTime() % 60);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
