using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AttributeMenu : MonoBehaviour
{
    // Number of available points
    public int availablePoints = 10;

    // TextMeshProUGUI for displaying available points
    public TextMeshProUGUI pointsText;

    public Health playerHealth;

    public PlayerController playerController;

    public PlayerAttackManager playerAttackManager;

    // Attribute categories
    public TextMeshProUGUI[] attributeTexts; // 6 attributes

    // Buttons
    public Button[] incrementButtons; // 6 increment buttons
    public Button[] decrementButtons; // 6 decrement buttons
    public Button resetButton;
    public Button buyButton;

    private int[] attributes = new int[6]; // Current attribute values
    private int[] confirmedAttributes = new int[6]; // Confirmed attribute values after buying
    private int[] initialAttributes;

    void Start()
    {
        // Initialize the UI
        UpdatePointsDisplay();
        UpdateAttributeTexts();

        initialAttributes = new int[attributes.Length];
        for (int i = 0; i < attributes.Length; i++)
        {
            initialAttributes[i] = attributes[i]; // Save the initial values
        }

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
        // Reset all values to their initial values and unlock all decrements
        for (int i = 0; i < attributes.Length; i++)
        {
            availablePoints += attributes[i]; // Return the spent points
            attributes[i] = initialAttributes[i]; // Restore original values
            confirmedAttributes[i] = 0; // Reset confirmed values
        }

        // Reset health and MAX_HEALTH to 300
        if (playerHealth != null)
        {
            playerHealth.health = 300; // Reset to original health value
            playerHealth.MAX_HEALTH = 300; // Reset to original max health value
        }

        if (playerController != null)
        {
            playerController.speed = playerController.originalSpeed; // Reset speed to original value
        }

        if (playerAttackManager != null)
        {
            playerAttackManager.damageModifier = 0; // Reset speed to original value
            playerAttackManager.ResetCooldowns(); // Reset cooldowns
        }

        UpdatePointsDisplay();
        UpdateAttributeTexts();
    }


    void LockAttributes()
    {
        // Confirm current values as baseline
        for (int i = 0; i < attributes.Length; i++)
        {
            int addedPoints = attributes[i] - confirmedAttributes[i];

            if (i == 0 && playerAttackManager != null)
            {
                playerAttackManager.damageModifier = attributes[0] * 5f;
            }
            else if (i == 1 && addedPoints > 0 && playerHealth != null)
            {
                playerHealth.IncreaseMaxStamina(addedPoints * 20f); // Each point = +20 stamina
            }
            else if (i == 2 && addedPoints > 0 && playerHealth != null)
            {
                playerHealth.IncreaseMaxHealth(addedPoints * 50);
            }
            else if (i == 3 && addedPoints > 0 && playerAttackManager != null)
            {
                // Decrease stamina cost by 1 per point
                float staminaReduction = addedPoints * 1f; // Subtract 1 for each added point
                foreach (var attack in playerAttackManager.attacks)
                {
                    attack.staminaUse = Mathf.Max(attack.staminaUse - staminaReduction, 0f); // Prevent stamina use from going negative
                }
            }
            else if (i == 4 && addedPoints > 0 && playerAttackManager != null)
            {
                // Decrease the cooldown by 0.1 per point
                float cooldownReduction = addedPoints * 0.1f; // Subtract 0.1 for each added point
                foreach (var attack in playerAttackManager.attacks)
                {
                    attack.cooldown = Mathf.Max(attack.cooldown - cooldownReduction, 0f); // Prevent cooldown from going negative
                }
            }

            else if (i == 5 && addedPoints > 0 && playerController != null)
            {
                playerController.IncreaseBaseSpeed(addedPoints * 2f);
            }

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