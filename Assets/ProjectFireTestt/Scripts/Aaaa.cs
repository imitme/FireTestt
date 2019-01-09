using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aaaa : MonoBehaviour
{
    public int score;
    public string nickname;
    public FB fb;

    public void OnAddScoreButtonClicked()
    {
        score = Random.Range(0, 100);
        nickname = "gkgk" + Random.Range(0, 100);

        fb.AddScore(score, nickname);
    }
}