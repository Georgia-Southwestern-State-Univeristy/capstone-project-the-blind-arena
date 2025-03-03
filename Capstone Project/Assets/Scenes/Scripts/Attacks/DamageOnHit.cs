using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    public int damageAmount;
    public float knockbackStrength;
    public bool detachFromPlayer;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Triggered with {other.name}");

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        Rigidbody enemyRb = other.GetComponent<Rigidbody>();
        FireBossAI fireBoss = other.GetComponent<FireBossAI>();

        if (enemyHealth != null && enemyRb != null)
        {
            enemyHealth.Damage(damageAmount);
            Debug.Log($"Dealt {damageAmount} damage to {other.name}");

            // Clear any existing velocity
            enemyRb.linearVelocity = Vector3.zero;

            Vector3 knockbackDirection;
            if (detachFromPlayer)
            {
                knockbackDirection = transform.forward;
            }
            else
            {
                knockbackDirection = (other.transform.position - transform.position);
            }

            knockbackDirection.y = 0;
            knockbackDirection = knockbackDirection.normalized;

            // Apply knockback only on X and Z axes
            Vector3 force = new Vector3(knockbackDirection.x, 0, knockbackDirection.z) * knockbackStrength;
            enemyRb.AddForce(force, ForceMode.VelocityChange);

            Debug.Log($"Applied Force: {force}");

            // Notify the FireBossAI that it's being knocked back
            if (fireBoss != null)
            {
                fireBoss.OnKnockback();
            }

            Destroy(gameObject);
        }
    }
}