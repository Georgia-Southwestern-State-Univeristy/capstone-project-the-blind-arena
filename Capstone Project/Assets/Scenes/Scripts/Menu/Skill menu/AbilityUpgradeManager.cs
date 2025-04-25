using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AbilityUpgradeManager : MonoBehaviour
{
    public static AbilityUpgradeManager Instance;

    [System.Serializable]
    public class AttackUpgrade
    {
        public string attackName;
        public float addedDamage;
        public float reducedCooldown;
        public float addedDuration;
        public float colliderSizeMultiplier = 1f;
        public float spriteSizeMultiplier = 1f;

        // Add more properties as needed
    }

    public List<AttackUpgrade> upgrades = new List<AttackUpgrade>();

    private List<AttackUpgrade> originalUpgrades = new List<AttackUpgrade>();

    private void Start()
    {
        SaveOriginalUpgrades(); // Store baseline values when the game starts
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public AttackUpgrade GetUpgrade(string attackName)
    {
        return upgrades.Find(u => u.attackName == attackName);
    }

    public void ApplyOrUpdateUpgrade(string attackName, float damage = 0, float cooldown = 0, float duration = 0)
    {
        var upgrade = GetUpgrade(attackName);
        if (upgrade == null)
        {
            upgrade = new AttackUpgrade
            {
                attackName = attackName,
                addedDamage = damage,
                reducedCooldown = cooldown,
                addedDuration = duration
            };
            upgrades.Add(upgrade);
        }
        else
        {
            upgrade.addedDamage += damage;
            upgrade.reducedCooldown += cooldown;
            upgrade.addedDuration += duration;
        }
    }

    public void ApplyOrUpdateSizeUpgrade(string attackName, float colliderMultiplier, float spriteMultiplier)
    {
        var upgrade = GetUpgrade(attackName);
        if (upgrade == null)
        {
            upgrade = new AttackUpgrade
            {
                attackName = attackName,
                colliderSizeMultiplier = colliderMultiplier,
                spriteSizeMultiplier = spriteMultiplier
            };
            upgrades.Add(upgrade);
        }
        else
        {
            upgrade.colliderSizeMultiplier *= colliderMultiplier;
            upgrade.spriteSizeMultiplier *= spriteMultiplier;
        }
    }

    public void SaveOriginalUpgrades()
    {
        originalUpgrades.Clear();
        foreach (var upgrade in upgrades)
        {
            var copy = new AttackUpgrade
            {
                attackName = upgrade.attackName,
                addedDamage = upgrade.addedDamage,
                reducedCooldown = upgrade.reducedCooldown,
                addedDuration = upgrade.addedDuration,
                colliderSizeMultiplier = upgrade.colliderSizeMultiplier,
                spriteSizeMultiplier = upgrade.spriteSizeMultiplier
            };
            originalUpgrades.Add(copy);
        }
    }

    public void ResetUpgradesToOriginal()
    {
        upgrades.Clear();
        foreach (var upgrade in originalUpgrades)
        {
            var copy = new AttackUpgrade
            {
                attackName = upgrade.attackName,
                addedDamage = upgrade.addedDamage,
                reducedCooldown = upgrade.reducedCooldown,
                addedDuration = upgrade.addedDuration,
                colliderSizeMultiplier = upgrade.colliderSizeMultiplier,
                spriteSizeMultiplier = upgrade.spriteSizeMultiplier
            };
            upgrades.Add(copy);
        }

        Debug.Log("Ability upgrades reset to original state.");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 2)
        {
            ResetUpgradesToOriginal(); // Only reset on scene index 2
        }
    }

}
