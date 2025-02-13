using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveData : MonoBehaviour
{
    public TMPro.TextMeshProUGUI myScore;
    public TMPro.TextMeshProUGUI myName;
    [SerializeField] private int currentScore; // Visible in Inspector

    private void Update()
    {
        // Extract the number from myScore.text and update currentScore
        UpdateScoreFromText();
    }

    private void UpdateScoreFromText()
    {
        if (myScore != null)
        {
            string scoreText = myScore.text.Replace("Score: ", ""); // Remove "Score: " prefix
            if (int.TryParse(scoreText, out int parsedScore))
            {
                currentScore = parsedScore; // Now Inspector will show the updated value
            }
        }
    }

    public void SendScore()
    {
        if (currentScore > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", currentScore);
            HighScores.UploadScore(myName.text, currentScore);
        }
    }
}
