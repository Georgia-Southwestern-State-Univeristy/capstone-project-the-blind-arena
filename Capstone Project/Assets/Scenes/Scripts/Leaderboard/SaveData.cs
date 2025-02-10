using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveData : MonoBehaviour
{
    public TMPro.TextMeshProUGUI myScore;
    public TMPro.TextMeshProUGUI myName;
    public int currentScore;

    private void Update()
    {
        myScore.text = $"Score: {PlayerPrefs.GetInt("highscore")}";
    }

    public void SendScore()
    {
        if (currentScore > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.GetInt("highscore", currentScore);
            HighScores.UploadScore(myName.text, currentScore);
        }
    }

}
