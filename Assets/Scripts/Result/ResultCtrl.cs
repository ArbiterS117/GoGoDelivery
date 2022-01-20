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
        ClearTimeText.text = ResultScore.GetFinalClearTime().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
