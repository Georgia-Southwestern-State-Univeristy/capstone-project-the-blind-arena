using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    public int damageAmount;
    public float knockbackStrength;
    public bool detachFromPlayer;

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        FireBossAI fireBoss = other.GetComponent<FireBossAI>();

        if (enemyHealth != null)
        {
            enemyHealth.Damage(damageAmount);

            if (fireBoss != null)
            {
                Vector3 knockbackDirection;
                Vector3 attackCenter = transform.position;

                // For detached attacks (like projectiles), use their forward direction
                if (detachFromPlayer)
                {
                    knockbackDirection = transform.forward;
                }
                // For attached attacks (like melee), push away from attack center
                else
                {
                    knockbackDirection = (other.transform.position - attackCenter);
                }

                // Ensure knockback is only horizontal (XZ plane)
                knockbackDirection.y = 0;
                knockbackDirection = knockbackDirection.normalized;

                // Calculate final knockback force with equal X and Z components
                Vector3 knockbackForce = new Vector3(
                    knockbackDirection.x * knockbackStrength,
                    0,
                    knockbackDirection.z * knockbackStrength
                );

                // Apply knockback force
                fireBoss.ApplyKnockback(knockbackForce);
            }

            if (!detachFromPlayer)
            {
                Destroy(gameObject);
            }
        }
    }
}
