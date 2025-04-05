using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MultiplayerHealth : NetworkBehaviour
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
        if (!IsOwner)
            return;

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
    }

    void Update()
    {
        if (!IsOwner)
            return;

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
        if (!IsOwner || amount < 0) throw new System.ArgumentOutOfRangeException("Cannot have negative Damage");

        int finalDamage = Mathf.CeilToInt((amount + 1) - takeDamageModifier);
        damageCollected += finalDamage;
        health -= finalDamage;
        UpdateHealthBar();

        if (health <= 0)
        {
            PlayerDie();
        }
    }

    public void Heal(int amount)
    {
        if (!IsOwner || amount < 0) throw new System.ArgumentOutOfRangeException("Cannot have negative Healing");

        health = Mathf.Min(health + amount, MAX_HEALTH);
        UpdateHealthBar();
    }

    public void UseStamina(float amount)
    {
        if (!IsOwner)
            return;

        stamina = Mathf.Max(stamina - amount, 0);
        UpdateStaminaBar();
        lastStaminaUseTime = Time.time; // Reset stamina regen delay
    }

    private void RegenerateStamina()
    {
        if (!IsOwner)
            return;

        if (stamina < MAX_STAMINA && Time.time > lastStaminaUseTime + staminaRegenDelay)
        {
            timeSinceLastRegen += Time.deltaTime;

            if (timeSinceLastRegen >= staminaRegenInterval)
            {
                stamina += staminaRegenRate;
                stamina = Mathf.Min(stamina, MAX_STAMINA);
                UpdateStaminaBar();
                timeSinceLastRegen = 0f;
            }
        }
        else
        {
            timeSinceLastRegen = 0f;
        }
    }

    private void UpdateHealthBar()
    {
        if (!IsOwner || healthBarSlider == null)
            return;

        healthBarSlider.value = health;
    }

    public void UpdateStaminaBar()
    {
        if (!IsOwner || staminaBarSlider == null)
            return;

        staminaBarSlider.value = Mathf.RoundToInt(stamina);
    }

    private void PlayerDie()
    {
        if (!IsOwner)
            return;

        Debug.Log($"{gameObject.name} has died.");

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SceneController.Instance.LoadScene(2);
        }
        else
        {
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
