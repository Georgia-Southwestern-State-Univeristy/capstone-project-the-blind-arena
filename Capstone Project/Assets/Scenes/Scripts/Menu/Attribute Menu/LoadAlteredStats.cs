using UnityEngine;

public class LoadAlteredStats : MonoBehaviour
{
    public Health playerHealth;
    public GameObject player;
    public PlayerController playerController;
    public PlayerAttackManager playerAttackManager;

    void Start()
    {
        var stats = PlayerStatsManager.Instance;

        if (playerHealth != null)
        {
            playerHealth.health = stats.health;
            playerHealth.MAX_HEALTH = stats.maxHealth;
            playerHealth.stamina = stats.stamina;
            playerHealth.MAX_STAMINA = stats.maxStamina;
        }

        if (playerController != null)
        {
            playerController.speed = stats.speed;
        }

        if (playerAttackManager != null)
        {
            playerAttackManager.damageModifier = stats.damageModifier;

            if (stats.attackCooldowns != null && stats.attackStaminaUses != null)
            {
                for (int i = 0; i < playerAttackManager.attacks.Length; i++)
                {
                    if (i < stats.attackCooldowns.Length)
                        playerAttackManager.attacks[i].cooldown = stats.attackCooldowns[i];

                    if (i < stats.attackStaminaUses.Length)
                        playerAttackManager.attacks[i].staminaUse = stats.attackStaminaUses[i];
                }
            }
        }

        // Optionally apply attribute points too
        var confirmed = stats.confirmedAttributes;
        // Use confirmed[i] to update abilities/UI/etc.


        if (playerAttackManager != null)
        {
            foreach (var attack in playerAttackManager.attacks)
            {
                var upgrade = AbilityUpgradeManager.Instance.GetUpgrade(attack.name);
                if (upgrade != null)
                {
                    attack.damage += upgrade.addedDamage;
                    attack.cooldown = Mathf.Max(attack.cooldown - upgrade.reducedCooldown, 0f);
                    attack.duration += upgrade.addedDuration;
                    attack.colliderSize *= upgrade.colliderSizeMultiplier;
                    attack.spriteSize *= upgrade.spriteSizeMultiplier;

                }
            }
        }


    }

}
