using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCtrl : MonoBehaviour
{
    public string CurrentScene;
    public GameObject TimeUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "Title"){
            if (Input.GetKeyDown(KeyCode.Return) ){
                SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
            }
        }

        if (scene.name == "Tutorial")
        {
            if (Input.GetKeyDown(KeyCode.Return) )
            {
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        }

        if (scene.name == "Tutorial2")
        {
            if (Input.GetKeyDown(KeyCode.Return) )
            {
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        }

        if (scene.name == "Game")
        {
            if (Input.GetKeyDown(KeyCode.F4) )
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                GameObject timeUI = TimeUI;
                ResultScore.SetFinalScore(player.GetComponent<player>().score);
                ResultScore.SetFinalClearTime(timeUI.GetComponent<TimeUI>().totaltime2);
                ResultScore.SetFinalDeathTime(player.GetComponent<player>().deathTime);
                SceneManager.LoadScene("Result", LoadSceneMode.Single);
            }
        }

        if (scene.name == "Result")
        {
            if (Input.GetKeyDown(KeyCode.Return) )
            {
                SceneManager.LoadScene("Title", LoadSceneMode.Single);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
}
