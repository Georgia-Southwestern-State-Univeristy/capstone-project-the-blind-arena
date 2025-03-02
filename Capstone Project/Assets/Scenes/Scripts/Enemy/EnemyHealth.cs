using UnityEngine;
using UnityEngine.UI;

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
        if (triggerSequenceOnDeath && objectToReveal != null && secondObjectToReveal != null)
        {
            ObjectSequenceManager.Instance.StartObjectSequence(objectToReveal, secondObjectToReveal, delayBeforeSwitch);
        }
        else
        {
            SceneController.Instance.LoadScene(3);
        }
        Destroy(gameObject);
    }
}