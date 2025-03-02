using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] public Slider healthBarSlider;

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
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHealthBar();

        if (currentHealth == 0)
            Die();
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
