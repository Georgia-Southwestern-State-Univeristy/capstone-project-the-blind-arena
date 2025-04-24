using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance;

    // Stats you can tweak
    public int[] confirmedAttributes = new int[6];
    public int availablePoints = 2;

    public float health = 300;
    public float maxHealth = 300;
    public float stamina = 100;
    public float maxStamina = 100;
    public float speed = 5;
    public float damageModifier = 0;
    public float[] attackCooldowns;
    public float[] attackStaminaUses;

    // Store original values for resetting
    private float originalHealth;
    private float originalMaxHealth;
    private float originalStamina;
    private float originalMaxStamina;
    private float originalSpeed;
    private float originalDamageModifier;
    private int originalAvailablePoints;
    private int[] originalAttributes = new int[6];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        SaveOriginalValues();
    }

    public void SaveOriginalValues()
    {
        originalHealth = health;
        originalMaxHealth = maxHealth;
        originalStamina = stamina;
        originalMaxStamina = maxStamina;
        originalSpeed = speed;
        originalDamageModifier = damageModifier;
        originalAvailablePoints = availablePoints;
        confirmedAttributes.CopyTo(originalAttributes, 0);
    }

    public void ResetToOriginalValues()
    {
        health = originalHealth;
        maxHealth = originalMaxHealth;
        stamina = originalStamina;
        maxStamina = originalMaxStamina;
        speed = originalSpeed;
        damageModifier = originalDamageModifier;
        availablePoints = originalAvailablePoints;
        originalAttributes.CopyTo(confirmedAttributes, 0);
    }

    public void ResetToDefaults()
    {
        health = 300;
        maxHealth = 300;
        stamina = 100;
        maxStamina = 100;
        speed = 5;
        damageModifier = 0;
        availablePoints = 2;
        attackCooldowns = null;
        attackStaminaUses = null;


        for (int i = 0; i < confirmedAttributes.Length; i++)
        {
            confirmedAttributes[i] = 0;
        }

        Debug.Log("Player stats reset to default values.");
    }

}
