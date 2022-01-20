using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultScore : MonoBehaviour
{
    public static int FinalScore;
    public static int FinalClearTime;
    public static int FinalDeathTime;

    public static void SetFinalScore(int score)
    {
        FinalScore = score;
    }

    public static int GetFinalScore()
    {
        return FinalScore;
    }

    public static void SetFinalClearTime(int ClearTime)
    {
        FinalClearTime = ClearTime;
    }

    public static int GetFinalClearTime()
    {
        return FinalClearTime;
    }

    public static void SetFinalDeathTime(int DeathTime)
    {
        FinalDeathTime = DeathTime;
    }

    public static int GetFinalDeathTime()
    {
        return FinalDeathTime;
    }

}
