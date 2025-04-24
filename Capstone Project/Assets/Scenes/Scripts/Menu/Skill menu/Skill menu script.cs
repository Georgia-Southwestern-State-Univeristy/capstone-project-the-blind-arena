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

    //List of attack modifications
    public List<float> attackModifiers;
    public PlayerAttackManager playerAttackManager;
    public PlayerController playerController;
    public Health health;

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

    // List of indices for abilities that cannot have points spent
    public List<int> restrictedAbilities;

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
            // Prevent spending points on restricted abilities
            if (restrictedAbilities.Contains(currentIndex))
            {
                Debug.Log("Points cannot be spent on this ability.");
                return;
            }

            // Reduce points
            points--;

            // Hide the lock image for the selected image
            var lockImage = lockImages[currentIndex];
            lockImage.gameObject.SetActive(false);

            // Apply attack modifier
            if (currentIndex == 1 && points > 0 && playerAttackManager != null)
            {
                foreach (var attack in playerAttackManager.attacks)
                {
                    if (attack.name == "RockThrow")
                    {
                        attack.damage += 2;
                    }
                }
            }

            if (currentIndex == 2 && points > 0 && playerAttackManager != null)
            {
                // Decrease the cooldown by 0.05 per point
                float cooldownReduction = points * 0.05f; // Subtract 0.1 for each point spent
                foreach (var attack in playerAttackManager.attacks)
                {
                    // Apply cooldown reduction for the specific attack
                    if (attack.name == "RockThrow") // assuming 4 is the ID for the attack you want to modify
                    {
                        attack.cooldown = Mathf.Max(attack.cooldown - cooldownReduction, 0f); 
                    }
                }
            }

            if (currentIndex == 3 && points > 0 && playerAttackManager != null)
            {
                foreach (var attack in playerAttackManager.attacks)
                {
                    if (attack.name == "RockThrow")
                    {
                         //change this for unique abilities changes
                    }
                }
            }

            if (currentIndex == 5 && points > 0 && playerAttackManager != null)
            {
                foreach (var attack in playerAttackManager.attacks)
                {
                    if (attack.name == "EarthWall") 
                    {
                        attack.duration += 2;
                    }
                }
            }

            if (currentIndex == 6 && points > 0 && playerAttackManager != null)
            {
                foreach (var attack in playerAttackManager.attacks)
                {
                    float cooldownReduction = points * 0.05f; // Subtract 0.1 for each point spent
                    // Apply cooldown reduction for the specific attack
                    if (attack.name == "EarthWall") //
                    {
                        attack.cooldown = Mathf.Max(attack.cooldown - cooldownReduction, 0f);
                    }
                }
            }

            if (currentIndex == 7 && points > 0 && playerAttackManager != null)
            {

                foreach (var attack in playerAttackManager.attacks)
                {
                    if (attack.name == "EarthWall") 
                    {
                         //change this for unique abilities changes
                    }
                }
            }

            if (currentIndex == 9 && points > 0 && playerController != null)
            {
                 
            }


            if (currentIndex == 10 && points > 0 && playerController != null)
            {

            }

            if (currentIndex == 11 && points > 0 && playerController != null)
            {

            }

            if (currentIndex == 13 && points > 0 && playerAttackManager != null)
            {
                foreach (var attack in playerAttackManager.attacks)
                {
                    if (attack.name == "EarthQuake")
                    {
                        attack.damage += 5;
                    }
                }
            }

            if (currentIndex == 14 && points > 0 && playerAttackManager != null)
            {
                foreach (var attack in playerAttackManager.attacks)
                {
                    // Apply cooldown reduction for the specific attack
                    if (attack.name == "EarthQuake") // assuming 4 is the ID for the attack you want to modify
                    {
                        attack.duration += 2;
                    }
                }
            }

            if (currentIndex == 15 && points > 0 && playerAttackManager != null)
            {

                foreach (var attack in playerAttackManager.attacks)
                {
                    if (attack.name == "EarthQuake")
                    {
                        //change this for unique abilities changes
                    }
                }
            }

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