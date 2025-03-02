using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI hoursText, minutesText, secondsText, millisecondsText;
    public TextMeshProUGUI counterText;
    public GameObject enemy; // Reference to the enemy game object

    private float elapsedTime = 0f;
    private bool isPaused = false;
    private bool isStopped = false;
    private float counter = 0f;
    public float counterIncrement = 1f; // Amount to add every second
    public float maxAmount = 10f; // Max amount for subtraction
    private float counterTimer = 0f;

    void Update()
    {
        if (!isPaused && !isStopped && enemy != null)
        {
            elapsedTime += Time.deltaTime;
            counterTimer += Time.deltaTime;

            if (counterTimer >= 1f)
            {
                counter += counterIncrement;
                counterTimer = 0f;
                UpdateCounterUI();
            }

            UpdateTimerUI();
        }
        else if (enemy == null && !isStopped)
        {
            StopTimerAndCalculateScore();
        }
    }

    void UpdateTimerUI()
    {
        int hours = (int)(elapsedTime / 3600);
        int minutes = (int)(elapsedTime % 3600) / 60;
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 100) % 100);

        hoursText.text = hours.ToString("00");
        minutesText.text = minutes.ToString("00");
        secondsText.text = seconds.ToString("00");
        millisecondsText.text = milliseconds.ToString("00");
    }

    void UpdateCounterUI()
    {
        counterText.text = counter.ToString();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
    }

    public void AddTime(float timeToAdd)
    {
        elapsedTime += timeToAdd;
        UpdateTimerUI();
    }

    void StopTimerAndCalculateScore()
    {
        isStopped = true;
        isPaused = true;
        counter = maxAmount - counter; // Subtract counter from max amount
        UpdateCounterUI();
    }

    public bool IsStopped()
    {
        return isStopped;
    }

    public string GetFormattedTime()
    {
        int hours = (int)(elapsedTime / 3600);
        int minutes = (int)(elapsedTime % 3600) / 60;
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 100) % 100);

        return $"{hours:00}:{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

}
