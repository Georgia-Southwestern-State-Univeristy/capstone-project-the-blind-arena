using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI hoursText, minutesText, secondsText, millisecondsText;

    private float elapsedTime = 0f;
    private bool isPaused = false;

    void Update()
    {
        if (!isPaused)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
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

    public void TogglePause()
    {
        isPaused = !isPaused;
    }

    public void AddTime(float timeToAdd)
    {
        elapsedTime += timeToAdd;
        UpdateTimerUI();
    }
}