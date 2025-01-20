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

    private int[] attributes = new int[6]; // Attribute values
    private bool pointsLocked = false; // If points are locked after buying

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
        if (!pointsLocked && availablePoints > 0)
        {
            attributes[index]++;
            availablePoints--;
            UpdatePointsDisplay();
            UpdateAttributeTexts();
        }
    }

    void DecrementAttribute(int index)
    {
        if (!pointsLocked && attributes[index] > 0)
        {
            attributes[index]--;
            availablePoints++;
            UpdatePointsDisplay();
            UpdateAttributeTexts();
        }
    }

    void ResetAttributes()
    {
        if (pointsLocked) pointsLocked = false;

        // Refund all points
        for (int i = 0; i < attributes.Length; i++)
        {
            availablePoints += attributes[i];
            attributes[i] = 0;
        }

        UpdatePointsDisplay();
        UpdateAttributeTexts();
    }

    void LockAttributes()
    {
        pointsLocked = true;
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
