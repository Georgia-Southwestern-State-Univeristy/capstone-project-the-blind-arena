using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] public Slider healthBarSlider;
    [SerializeField] private bool triggerSequenceOnDeath = false;
    [SerializeField] private GameObject objectToReveal;
    [SerializeField] private GameObject secondObjectToReveal;
    [SerializeField] private float delayBeforeSwitch = 3f;
    private VisibilityObjects visibilityController;
    private VisibilityObjects secondVisibilityController;

    // New fields for sprite flash
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = Color.red;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
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

        // Get sprite renderer for flash effect
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Damage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHealthBar();

        // Flash sprite red when damaged
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashSprite());
        }

        if (currentHealth == 0)
            Die();
    }

    private IEnumerator FlashSprite()
    {
        // Store original color
        Color originalColor = spriteRenderer.color;

        // Change to flash color
        spriteRenderer.color = flashColor;

        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);

        // Restore original color
        spriteRenderer.color = originalColor;
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        if (triggerSequenceOnDeath && objectToReveal != null && secondObjectToReveal != null)
        {
            ObjectSequenceManager.Instance.StartObjectSequence(objectToReveal, secondObjectToReveal, delayBeforeSwitch);
        }
        else
        {
            SceneController.Instance.LoadScene(2);
        }
        Destroy(gameObject);
    }

    public void Respawn()
    {
        SceneController.Instance.LoadScene(2);
    }

    public void TakeBackToMenu()
    {
        SceneController.Instance.LoadScene(0);
    }
}