using UnityEngine;
using TMPro;

public class TimerCopy : MonoBehaviour
{
    public GameTimer gameTimer; // Reference to the GameTimer script
    public TextMeshProUGUI finalTimeText; // Reference to the UI Text to display final time
    private bool isCopied = false;

    void Update()
    {
        if (!isCopied && gameTimer != null && gameTimer.IsStopped())
        {
            CopyFinalTime();
            isCopied = true;
        }
    }

    void CopyFinalTime()
    {
        if (gameTimer != null && finalTimeText != null)
        {
            string finalTime = gameTimer.GetFormattedTime();
            finalTimeText.text = "Final Time: " + finalTime;
        }
    }
}
