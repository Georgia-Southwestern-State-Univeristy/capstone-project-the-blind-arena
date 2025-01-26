using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public GameObject tenMinuteObject;  // GameObject for 10-minute section
    public GameObject oneMinuteObject; // GameObject for 1-minute section
    public GameObject tenSecondObject; // GameObject for 10-second section
    public GameObject oneSecondObject; // GameObject for 1-second section

    public Button buyButton;   // Button to add time
    public float purchaseTime = 30f; // Time to add on purchase

    private Text tenMinuteText;
    private Text oneMinuteText;
    private Text tenSecondText;
    private Text oneSecondText;

    private float timer = 0f;  // Total time in seconds
    private bool isInRestArea = false;

    void Start()
    {
        // Get the Text components from the GameObjects
        tenMinuteText = tenMinuteObject.GetComponent<Text>();
        oneMinuteText = oneMinuteObject.GetComponent<Text>();
        tenSecondText = tenSecondObject.GetComponent<Text>();
        oneSecondText = oneSecondObject.GetComponent<Text>();

        // Add a listener to the buy button
        buyButton.onClick.AddListener(AddPurchaseTime);
    }

    void Update()
    {
        if (!isInRestArea)
        {
            timer += Time.deltaTime; // Count up
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        // Calculate the time sections
        int totalSeconds = Mathf.FloorToInt(timer);
        int tenMinutes = (totalSeconds / 600) % 10;
        int oneMinutes = (totalSeconds / 60) % 10;
        int tenSeconds = (totalSeconds / 10) % 6;
        int oneSeconds = totalSeconds % 10;

        // Update the UI
        tenMinuteText.text = tenMinutes.ToString();
        oneMinuteText.text = oneMinutes.ToString();
        tenSecondText.text = tenSeconds.ToString();
        oneSecondText.text = oneSeconds.ToString();
    }

    public void EnterRestArea()
    {
        isInRestArea = true;
    }

    public void ExitRestArea()
    {
        isInRestArea = false;
    }

    private void AddPurchaseTime()
    {
        timer += purchaseTime;
        UpdateTimerDisplay(); // Update the timer after adding time
    }
}