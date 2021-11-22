using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(UpgradeInformation))]
public class UpgradeInformationEditor : Editor
{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        UpgradeInformation upgradeInformation = (UpgradeInformation)target;

        upgradeInformation.Type = (UpgradeInformation.UpgradeType)EditorGUILayout.EnumPopup("Item Type", upgradeInformation.Type);

        switch (upgradeInformation.Type)
        {
            case UpgradeInformation.UpgradeType.SpeedUp:
                upgradeInformation.SpeedUpSpeed = EditorGUILayout.Slider("SpeedUp Speed", upgradeInformation.SpeedUpSpeed, 0.0f, 100.0f);
                break;

            case UpgradeInformation.UpgradeType.AirJump:
                upgradeInformation.AirJumpForce = EditorGUILayout.Slider("AirJump Force", upgradeInformation.AirJumpForce, 0.0f, 3000.0f);
                break;

            case UpgradeInformation.UpgradeType.AirDash:
                upgradeInformation.AirDashSpeed = EditorGUILayout.Slider("AirDash Speed", upgradeInformation.AirDashSpeed, 0.0f, 100.0f);
                break;

            case UpgradeInformation.UpgradeType.Grapple:
                upgradeInformation.GrappleLen = EditorGUILayout.Slider("Grapple Len", upgradeInformation.GrappleLen, 0.0f, 1000.0f);
                break;

        }

        
    }
}

