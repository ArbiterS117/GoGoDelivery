using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public bool xx = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        xx = true;
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(OnTriggerEnter)
        //    SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
