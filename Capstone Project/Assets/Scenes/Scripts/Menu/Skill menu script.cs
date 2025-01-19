using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HighlightImageOnClick : MonoBehaviour
{
    // List of images to manage
    public List<Image> images;

    // List of associated TextMeshPro texts
    public List<TextMeshProUGUI> texts;

    // Lock images to hide when buying
    public List<Image> lockImages;

    // Highlight color for the outline
    public Color highlightColor = Color.yellow;

    // Thickness of the outline
    public float outlineThickness = 2f;

    // Player points
    public int points = 10;

    // TextMeshProUGUI for displaying points
    public TextMeshProUGUI pointsText;

    // Button for the buy action
    public Button buyButton;

    private Outline currentHighlighted;
    private TextMeshProUGUI currentVisibleText;
    private int currentIndex = -1;

    void Start()
    {
        // Ensure each image has a Button component for click detection
        for (int i = 0; i < images.Count; i++)
        {
            var localIndex = i; // Capture index for closure
            var image = images[localIndex];
            var button = image.GetComponent<Button>();
            if (button == null)
            {
                button = image.gameObject.AddComponent<Button>();
            }

            // Add click listener
            button.onClick.AddListener(() => OnImageClicked(localIndex));
        }

        // Assign the buy button click listener
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }

        // Hide all texts initially
        foreach (var text in texts)
        {
            text.gameObject.SetActive(false);
        }

        // Update the points display
        UpdatePointsDisplay();
    }

    void OnImageClicked(int index)
    {
        // Remove highlight from previously highlighted image
        if (currentHighlighted != null)
        {
            Destroy(currentHighlighted);
        }

        // Hide previously visible text
        if (currentVisibleText != null)
        {
            currentVisibleText.gameObject.SetActive(false);
        }

        // Add highlight to the clicked image
        var clickedImage = images[index];
        var outline = clickedImage.gameObject.AddComponent<Outline>();
        outline.effectColor = highlightColor;
        outline.effectDistance = new Vector2(outlineThickness, outlineThickness);
        currentHighlighted = outline;

        // Show the associated text
        var associatedText = texts[index];
        associatedText.gameObject.SetActive(true);
        currentVisibleText = associatedText;

        // Update current index
        currentIndex = index;
    }

    public void OnBuyButtonClicked()
    {
        // Check if a valid image is selected and points are greater than zero
        if (currentIndex != -1 && points > 0)
        {
            // Reduce points
            points--;

            // Hide the lock image for the selected image
            var lockImage = lockImages[currentIndex];
            lockImage.gameObject.SetActive(false);

            // Update the points display
            UpdatePointsDisplay();
        }

        // Disable further buying if points reach zero
        if (points <= 0)
        {
            DisableAllButtons();
        }
    }

    private void UpdatePointsDisplay()
    {
        if (pointsText != null)
        {
            pointsText.text = "Points: " + points;
        }
    }

    private void DisableAllButtons()
    {
        foreach (var image in images)
        {
            var button = image.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }
        }

        if (buyButton != null)
        {
            buyButton.interactable = false;
        }
    }
}