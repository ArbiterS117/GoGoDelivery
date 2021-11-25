using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeInformation : MonoBehaviour
{
   
    public enum UpgradeType
    {
        SpeedUp,
        AirJump,
        AirDash,
        Grapple,
    }

    [Header("Upgrade Type")]
    public UpgradeType Type;
    [Space(10)]

    [Header("Upgrade Information 2")]
     public int    Num;

    [Range(5f, 50f)]
    public float  SpeedUpSpeed;

    [Tooltip("force value should be between 5 and 500. Maximum force should be based on the category of the Item.")]
    [Range(5f, 500f)]
    public float  AirJumpForce;

    public float AirDashSpeed;

    public float GrappleLen;

    [ContextMenu("DifficultyNoob")]


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}

