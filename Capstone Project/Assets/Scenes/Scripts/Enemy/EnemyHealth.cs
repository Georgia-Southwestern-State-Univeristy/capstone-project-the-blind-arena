using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] public Slider healthBarSlider;
    [SerializeField] public bool triggerSequenceOnDeath = false;
    [SerializeField] private GameObject objectToReveal;
    [SerializeField] private GameObject secondObjectToReveal;
    [SerializeField] private float delayBeforeSwitch = 3f;
    private VisibilityObjects visibilityController;
    private VisibilityObjects secondVisibilityController;

    // New fields for sprite flash
    private SpriteRenderer spriteRenderer;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Material hold;
    private bool flashLock;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private AudioSource flashSound;
    [SerializeField] private AudioSource victorySound;
    public double deathcounter;

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
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    public void Damage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHealthBar();

        // Flash sprite red when damaged
        if (spriteRenderer != null && amount > 0)
        {
            StartCoroutine(FlashSprite());
        }

        if (currentHealth == 0)
            Die();
    }

    private IEnumerator FlashSprite()
    {
        if (!flashLock) {
            flashLock=true;

            if (flashSound != null)
            {
                flashSound.Play(); // Play the sound at the start of the flash
            }

            for (int i = 0; i < 2; i++)
            {
                // Store original color
                hold = spriteRenderer.material;
                spriteRenderer.material = skinnedMeshRenderer.material;
                Color originalColor = spriteRenderer.color;

                Debug.Log("Sprite Material = " + hold);
                Debug.Log("Other Material = " + skinnedMeshRenderer.material);

                // Change to flash color
                spriteRenderer.color = flashColor;

                // Wait for flash duration
                yield return new WaitForSeconds(flashDuration);

                // Restore original color
                spriteRenderer.color = originalColor;
                spriteRenderer.material = hold;
            }
            flashLock = false;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = currentHealth;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");

        if (gameObject.CompareTag("Boss"))
        {
            foreach (AudioSource source in FindObjectsOfType<AudioSource>())
            {
                if (source != victorySound)
                {
                    source.Stop();
                }



            victorySound.Play(); // Play the sound at the start of the flash

            }
            Destroy(gameObject);
            GameData.deathcounter++;
            SceneController.Instance.LoadScene(2);
            return; // Exit function to prevent further execution
        }

        // Check if it's the player dying
        if (gameObject.CompareTag("Player"))
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;

            // Destroy the object before transitioning
            Destroy(gameObject);

            // If the player dies in scene 1, load scene 2
            if (currentScene == 1)
            {
                SceneController.Instance.LoadScene(2);
            }
            else
            {
                // Handle object sequences if applicable
                if (triggerSequenceOnDeath && objectToReveal != null && secondObjectToReveal != null)
                {
                    ObjectSequenceManager.Instance.StartObjectSequence(objectToReveal, secondObjectToReveal, delayBeforeSwitch);
                }
                else
                {
                    SceneController.Instance.LoadScene(2);
                }
            }
        }
        else
        {
            // Handle enemy deaths
            if (triggerSequenceOnDeath && objectToReveal != null && secondObjectToReveal != null)
            {
                ObjectSequenceManager.Instance.StartObjectSequence(objectToReveal, secondObjectToReveal, delayBeforeSwitch);
            }
            else
            {
                GameData.deathcounter++;
            }

            // Destroy enemy object
            Destroy(gameObject);
        }
    }
}