using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCtrl : MonoBehaviour
{
    public string CurrentScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "Title"){
            if (Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.F1)){
                SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
            }
        }

        if (scene.name == "Tutorial")
        {
            if (Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.F1))
            {
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        }

        if (scene.name == "Game")
        {
            if (Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.F1))
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                ResultScore.SetFinalScore(player.GetComponent<player>().score);
                SceneManager.LoadScene("Result", LoadSceneMode.Single);
            }
        }

        if (scene.name == "Result")
        {
            if (Input.GetKeyDown(KeyCode.Return) | Input.GetKeyDown(KeyCode.F1))
            {
                SceneManager.LoadScene("Title", LoadSceneMode.Single);
            }
        }
    }
}
