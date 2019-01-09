using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    public Text scoreText;
    public Text nicknameText;
    public Text timestampText;
    private int score;

    public int Score
    {
        get { return score; }
        set { score = value; scoreText.text = value.ToString(); }
    }

    private string nickname;

    public string Nickname
    {
        get { return nickname; }
        set { nickname = value; nicknameText.text = value; }
    }

    private string timestamp;

    public string Timestamp
    {
        get { return timestamp; }
        set { timestamp = value; timestampText.text = value; }
    }

    public void Reset()
    {
        Score = 0;
        Nickname = string.Empty;
        Timestamp = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
    }
}