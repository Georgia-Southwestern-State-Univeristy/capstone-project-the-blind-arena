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
        UpdateScoreFromText();
    }

    private void UpdateScoreFromText()
    {
        if (myScore != null)
        {
            string scoreText = myScore.text.Replace("Score: ", "").Trim(); // Remove prefix
            if (int.TryParse(scoreText, out int parsedScore))
            {
                currentScore = parsedScore;
            }
        }
    }

    public void SendScore()
    {
        UpdateScoreFromText(); // Ensure currentScore is correct

        // Always send the score, no matter what
        HighScores.UploadScore(myName.text, currentScore);

        // Update high score if it's higher
        int highScore = PlayerPrefs.GetInt("highscore", 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("highscore", currentScore);
            PlayerPrefs.Save();
        }
    }
}
