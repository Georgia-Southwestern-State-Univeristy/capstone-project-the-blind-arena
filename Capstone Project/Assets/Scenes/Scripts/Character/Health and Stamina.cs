using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private Slider healthBarSlider; // Health bar UI
    [SerializeField] private Slider staminaBarSlider; // Stamina bar UI

    private int MAX_HEALTH = 100;
    private int MAX_STAMINA = 100;
    private int attackCost = 20; // Stamina cost for attacking
    private float staminaRegenRate = 5f; // Stamina regenerates by 5 per second
    private float staminaRegenDelay = 2f; // Delay before stamina starts regenerating
    private float lastStaminaUseTime; // Tracks last time stamina was used

    private float stamina; // Using float for accurate regen calculations

    void Start()
    {
        stamina = MAX_STAMINA; // Initialize stamina with float value

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

    private void UseStamina(int amount)
    {
        stamina = Mathf.Max(stamina - amount, 0);
        UpdateStaminaBar();
        lastStaminaUseTime = Time.time; // Reset stamina regen delay
    }

    private void RegenerateStamina()
    {
        if (stamina < MAX_STAMINA && Time.time > lastStaminaUseTime + staminaRegenDelay)
        {
            stamina += staminaRegenRate * Time.deltaTime; // Gradual regeneration
            stamina = Mathf.Min(stamina, MAX_STAMINA); // Prevent overfill
            UpdateStaminaBar();
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