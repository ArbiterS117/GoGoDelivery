using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlackToEnd : MonoBehaviour
{
    public GameObject TimeUI;

    public void changeScene()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject timeUI = TimeUI;
        ResultScore.SetFinalScore(player.GetComponent<player>().score);
        ResultScore.SetFinalClearTime(timeUI.GetComponent<TimeUI>().totaltime2);
        ResultScore.SetFinalDeathTime(player.GetComponent<player>().deathTime);
        SceneManager.LoadScene("Result", LoadSceneMode.Single);
    }
}
