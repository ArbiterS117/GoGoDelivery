using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultScore : MonoBehaviour
{
    public static int FinalScore;

    public static void SetFinalScore(int score)
    {
        FinalScore = score;
    }

    public static int GetFinalScore()
    {
        return FinalScore;
    }

}
