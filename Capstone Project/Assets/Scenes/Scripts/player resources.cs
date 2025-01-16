using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Image healthBar; // Assign a UI Image representing the health bar
    public AudioClip damageSound; // Assign a damage sound in the Inspector
    private AudioSource audioSource;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    private float currentStamina;
    public Image staminaBar; // Assign a UI Image representing the stamina bar
    public float staminaRegenRate = 5f; // Stamina regenerated per second
    public float dashStaminaCost = 20f;
    public float abilityStaminaCost = 30f;

    [Header("Damage Feedback")]
    public Renderer playerRenderer; // Assign the player's material renderer
    public Color damageColor = Color.red; // Color to flash when damaged
    public float flashDuration = 0.2f;
    private Color originalColor;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (playerRenderer != null)
            originalColor = playerRenderer.material.color;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Regenerate stamina over time
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            UpdateStaminaBar();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        // Play damage sound
        if (audioSource != null && damageSound != null)
            audioSource.PlayOneShot(damageSound);

        // Flash red
        if (playerRenderer != null)
            StartCoroutine(FlashDamage());

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;
    }

    private void UpdateStaminaBar()
    {
        if (staminaBar != null)
            staminaBar.fillAmount = currentStamina / maxStamina;
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            UpdateStaminaBar();
            return true; // Action allowed
        }
        return false; // Not enough stamina
    }

    private IEnumerator FlashDamage()
    {
        playerRenderer.material.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        playerRenderer.material.color = originalColor;
    }

    public void Dash()
    {
        if (UseStamina(dashStaminaCost))
        {
            Debug.Log("Player dashes!");
            // Add your dash logic here
        }
        else
        {
            Debug.Log("Not enough stamina to dash!");
        }
    }

    public void UseAbility()
    {
        if (UseStamina(abilityStaminaCost))
        {
            Debug.Log("Player uses an ability!");
            // Add your ability logic here
        }
        else
        {
            Debug.Log("Not enough stamina to use ability!");
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player has died!");
        // Add death handling logic (e.g., respawn, game over screen)
    }
}
