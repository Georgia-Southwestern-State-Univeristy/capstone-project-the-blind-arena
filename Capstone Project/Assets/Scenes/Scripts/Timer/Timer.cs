using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI hoursText, minutesText, secondsText, millisecondsText;
    public TextMeshProUGUI counterText;
    public GameObject enemy; // Reference to the enemy game object

    // Static variables to persist across scenes
    private static float persistentElapsedTime = 0f;
    private static float persistentCounter = 0f;
    private static bool hasUpdatedTime = false;

    private float elapsedTime = 0f;
    private bool isPaused = false;
    private bool isStopped = false;
    private float counter = 0f;
    public float counterIncrement = 1f; // Amount to add every second
    public float maxAmount = 10f; // Max amount for subtraction
    private float counterTimer = 0f;
    public float respawnPenalty = 30f; // Time penalty added on respawn

    void Start()
    {
        if (persistentElapsedTime > 0)
        {
            elapsedTime = persistentElapsedTime;
            counter = persistentCounter;
            UpdateTimerUI();
            UpdateCounterUI();
        }
    }


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

            // Persist elapsed time across fights
            persistentElapsedTime = elapsedTime;
            persistentCounter = counter;
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

        // Store the updated time persistently
        persistentElapsedTime = elapsedTime;
        persistentCounter = counter;


        UpdateTimerUI();
    }

    void StopTimerAndCalculateScore()
    {
        isStopped = true;
        isPaused = true;

        // Store the current time before stopping
        persistentElapsedTime = elapsedTime;
        persistentCounter = counter;


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

    public float GetCurrentTime()
    {
        return elapsedTime;
    }

    public void SetTime(float time)
    {
        elapsedTime = time;
        UpdateTimerUI();
    }

    public void Respawn()
    {
        // Apply respawn penalty
        elapsedTime += respawnPenalty;
        counter += respawnPenalty; // Add penalty to counter
        persistentElapsedTime = elapsedTime;
        persistentCounter = counter;
        UpdateTimerUI();
        StopTimerAndCalculateScore(); // Recalculate score after respawn
        SceneController.Instance.LoadScene(2);
    }

    public void TakeBackToMenu()
    {
        SceneController.Instance.LoadScene(0);
    }

}
