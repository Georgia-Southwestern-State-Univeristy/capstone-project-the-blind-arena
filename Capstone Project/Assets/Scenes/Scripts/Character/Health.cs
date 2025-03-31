using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] public int health = 300;
    [SerializeField] public float stamina = 100f;
    [SerializeField] private Slider healthBarSlider; // Health bar UI
    [SerializeField] private Slider staminaBarSlider; // Stamina bar UI

    private int MAX_HEALTH = 300;
    public float MAX_STAMINA = 100f;
    private int staminaRegenRate = 9; // Stamina regenerates by 1 per 0.3 seconds
    private float staminaRegenDelay = 2f; // Delay before stamina starts regenerating
    private float lastStaminaUseTime; // Tracks last time stamina was used
    private float damageMultiplier = 1f; // Default to 100% damage taken

    // New variables for timer-based regeneration
    private float staminaRegenInterval = 0.3f; // Interval for stamina regeneration (0.3 seconds)
    private float timeSinceLastRegen = 0f; // Timer to track regeneration intervals

    [SerializeField] private bool triggerSequenceOnDeath = false;
    [SerializeField] private GameObject objectToReveal;
    [SerializeField] private GameObject secondObjectToReveal;
    [SerializeField] private float delayBeforeSwitch = 3f;
    private VisibilityObjects visibilityController;
    private VisibilityObjects secondVisibilityController;

    public float takeDamageModifier = 0f;
    public int damageCollected = 0;

    void Start()
    {
        MAX_HEALTH = health;
        stamina = MAX_STAMINA;

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = MAX_HEALTH;
            healthBarSlider.value = health;
        }

        if (staminaBarSlider != null)
        {
            staminaBarSlider.maxValue = MAX_STAMINA;
            staminaBarSlider.value = stamina;
        }
        if (objectToReveal != null)
        {
            visibilityController = objectToReveal.GetComponent<VisibilityObjects>();
            if (visibilityController == null)
            {
                visibilityController = objectToReveal.AddComponent<VisibilityObjects>();
            }
            visibilityController.SetVisibility(false);
        }

        if (secondObjectToReveal != null)
        {
            secondVisibilityController = secondObjectToReveal.GetComponent<VisibilityObjects>();
            if (secondVisibilityController == null)
            {
                secondVisibilityController = secondObjectToReveal.AddComponent<VisibilityObjects>();
            }
            secondVisibilityController.SetVisibility(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Damage(10);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Heal(10);
        }
        RegenerateStamina();
    }

    public void Damage(int amount)
    {
        if (amount < 0) throw new System.ArgumentOutOfRangeException("Cannot have negative Damage");

        int finalDamage = Mathf.CeilToInt((amount + 1) - takeDamageModifier);
        damageCollected += finalDamage;
        health -= finalDamage;
        UpdateHealthBar();

        if (health <= 0)
        {
            PlayerDie();
        }
    }

    public void AdjustTakeDamage(float takeDamageChange, float duration)
    {
        StartCoroutine(AdjustTakeDamageCoroutine(takeDamageChange, duration));
    }

    private IEnumerator AdjustTakeDamageCoroutine (float takeDamageChange, float duration)
    {
        takeDamageModifier += takeDamageChange; //decrease damage taken
        yield return new WaitForSeconds(duration);
        takeDamageModifier -= takeDamageChange; //revert damage back to normal
    }


    public void Heal(int amount)
    {
        if (amount < 0) throw new System.ArgumentOutOfRangeException("Cannot have negative Healing");

        health = Mathf.Min(health + amount, MAX_HEALTH);
        UpdateHealthBar();
    }

    public void UseStamina(float amount)
    {
        stamina = Mathf.Max(stamina - amount, 0);
        UpdateStaminaBar();
        lastStaminaUseTime = Time.time; // Reset stamina regen delay
    }

    private void RegenerateStamina()
    {
        // Only regenerate if stamina is less than MAX and enough time has passed since the last regen
        if (stamina < MAX_STAMINA && Time.time > lastStaminaUseTime + staminaRegenDelay)
        {
            timeSinceLastRegen += Time.deltaTime;

            if (timeSinceLastRegen >= staminaRegenInterval)
            {
                // Regenerate stamina by the defined rate
                stamina += staminaRegenRate;
                stamina = Mathf.Min(stamina, MAX_STAMINA); // Prevent overfill

                UpdateStaminaBar();

                // Reset the timer after each regen cycle
                timeSinceLastRegen = 0f;
            }
        }
        else
        {
            // Reset the regen timer if stamina was recently used
            timeSinceLastRegen = 0f;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
        }
    }

    public void UpdateStaminaBar()
    {
        if (staminaBarSlider != null)
        {
            staminaBarSlider.value = Mathf.RoundToInt(stamina); // Convert float to int for UI
        }
    }

    private void PlayerDie()
    {
        Debug.Log($"{gameObject.name} has died.");

        // Check if we're in scene index 1
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            // If we're in scene 1, load scene 2
            SceneController.Instance.LoadScene(2);
        }
        else
        {
            // Otherwise, proceed with the original death behavior
            if (triggerSequenceOnDeath && objectToReveal != null && secondObjectToReveal != null)
            {
                ObjectSequenceManager.Instance.StartObjectSequence(objectToReveal, secondObjectToReveal, delayBeforeSwitch);
            }
            else
            {
                SceneController.Instance.LoadScene(2);
            }
        }

        Destroy(gameObject);
    }
}
