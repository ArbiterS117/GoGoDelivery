using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WarringBlink : MonoBehaviour
{
    SpriteRenderer sprite;
    public float time = 1.0f;
           float ctime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ctime += Time.deltaTime;
        if(ctime < time) sprite.DOFade(0.0f, 1.0f);
        else if(ctime < 2 * time)
        {
            sprite.DOFade(1.0f, 1.0f);
        }
        else
        {
            ctime = 0.0f;
        }
        
        
    }
}
