using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] public int health = 100;
    [SerializeField] public int stamina = 100;
    [SerializeField] private Slider healthBarSlider; // Health bar UI
    [SerializeField] private Slider staminaBarSlider; // Stamina bar UI

    private int MAX_HEALTH = 100;
    private int MAX_STAMINA = 100;
    private int attackCost = 20; // Stamina cost for attacking
    private int staminaRegenRate = 5; // Stamina regenerates by 1 per 0.3 seconds
    private float staminaRegenDelay = 3f; // Delay before stamina starts regenerating
    private float lastStaminaUseTime; // Tracks last time stamina was used

    // New variables for timer-based regeneration
    private float staminaRegenInterval = 0.3f; // Interval for stamina regeneration (0.3 seconds)
    private float timeSinceLastRegen = 0f; // Timer to track regeneration intervals

    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Damage(10);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Heal(10);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        RegenerateStamina();
    }

    public void Damage(int amount)
    {
        if (amount < 0) throw new System.ArgumentOutOfRangeException("Cannot have negative Damage");

        health -= amount;
        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount < 0) throw new System.ArgumentOutOfRangeException("Cannot have negative Healing");

        health = Mathf.Min(health + amount, MAX_HEALTH);
        UpdateHealthBar();
    }

    private void Attack()
    {
        if (stamina >= attackCost)
        {
            Debug.Log("Player attacked!");
            UseStamina(attackCost);
        }
        else
        {
            Debug.Log("Not enough stamina to attack!");
        }
    }

    public void UseStamina(int amount)
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
        } else {
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

    private void UpdateStaminaBar()
    {
        if (staminaBarSlider != null)
        {
            staminaBarSlider.value = Mathf.RoundToInt(stamina); // Convert float to int for UI
        }
    }

    private void Die()
    {
        Debug.Log("I have died");
        Destroy(gameObject);
    }
}
