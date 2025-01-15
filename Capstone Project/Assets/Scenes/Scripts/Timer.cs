using UnityEngine;
using UnityEngine.UI; // Only needed if you're displaying the timer in a UI text element

public class Timer : MonoBehaviour
{
    public Text timerText; // Assign a UI Text element in the inspector if you want to display the timer

    private float elapsedTime;
    private bool isRunning;

    void Start()
    {
        elapsedTime = 0f;
        isRunning = true; // Starts the timer automatically; set to false if you want manual control
    }

    void Update()
    {
        if (isRunning)
        {
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Optionally display the timer in a UI Text element
            if (timerText != null)
            {
                timerText.text = FormatTime(elapsedTime);
            }
        }
    }

    // Formats the time as minutes:seconds:milliseconds
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    // Public methods to control the timer
    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        if (timerText != null)
        {
            timerText.text = FormatTime(elapsedTime);
        }
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}