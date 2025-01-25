using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttributeMenu : MonoBehaviour
{
    // Number of available points
    public int availablePoints = 10;

    // TextMeshProUGUI for displaying available points
    public TextMeshProUGUI pointsText;

    // Attribute categories
    public TextMeshProUGUI[] attributeTexts; // 6 attributes

    // Buttons
    public Button[] incrementButtons; // 6 increment buttons
    public Button[] decrementButtons; // 6 decrement buttons
    public Button resetButton;
    public Button buyButton;

    private int[] attributes = new int[6]; // Current attribute values
    private int[] confirmedAttributes = new int[6]; // Confirmed attribute values after buying

    void Start()
    {
        // Initialize the UI
        UpdatePointsDisplay();
        UpdateAttributeTexts();

        // Assign button listeners
        for (int i = 0; i < 6; i++)
        {
            int index = i; // Capture index for closure
            incrementButtons[i].onClick.AddListener(() => IncrementAttribute(index));
            decrementButtons[i].onClick.AddListener(() => DecrementAttribute(index));
        }

        resetButton.onClick.AddListener(ResetAttributes);
        buyButton.onClick.AddListener(LockAttributes);
    }

    void IncrementAttribute(int index)
    {
        if (availablePoints > 0)
        {
            attributes[index]++;
            availablePoints--;
            UpdatePointsDisplay();
            UpdateAttributeTexts();
        }
    }

    void DecrementAttribute(int index)
    {
        // Allow decrement only if the attribute value is greater than the confirmed value
        if (attributes[index] > confirmedAttributes[index])
        {
            attributes[index]--;
            availablePoints++;
            UpdatePointsDisplay();
            UpdateAttributeTexts();
        }
    }

    void ResetAttributes()
    {
        // Reset all values back to zero and unlock all decrements
        for (int i = 0; i < attributes.Length; i++)
        {
            availablePoints += attributes[i];
            attributes[i] = 0;
            confirmedAttributes[i] = 0; // Reset confirmed values
        }

        UpdatePointsDisplay();
        UpdateAttributeTexts();
    }

    void LockAttributes()
    {
        // Confirm current values as baseline
        for (int i = 0; i < attributes.Length; i++)
        {
            confirmedAttributes[i] = attributes[i];
        }
    }

    void UpdatePointsDisplay()
    {
        pointsText.text = "Points: " + availablePoints;
    }

    void UpdateAttributeTexts()
    {
        for (int i = 0; i < attributeTexts.Length; i++)
        {
            attributeTexts[i].text = attributes[i].ToString();
        }
    }
}