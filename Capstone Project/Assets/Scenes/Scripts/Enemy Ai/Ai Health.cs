using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private Slider healthBarSlider;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }
    }

    public void Damage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            AiDie();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }
    }

    private void AiDie()
    {
        Debug.Log("Enemy has died!");
        Destroy(gameObject);
    }

    // Detects dynamically spawned attacks
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack")) // Make sure spawned attack has this tag
        {
            AttackData attack = collision.GetComponent<AttackData>();
            if (attack != null)
            {
                Damage(attack.damageAmount);
                Destroy(collision.gameObject); // Remove attack object after hit
            }
        }
    }
}
