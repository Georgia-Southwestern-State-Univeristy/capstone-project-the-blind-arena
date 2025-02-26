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

        if (enemyHealth != null)
        {
            enemyHealth.Damage(damageAmount);
            Debug.Log($"Dealt {damageAmount} damage to {other.name}");

            if (enemyRb != null)
            {
                Vector3 knockbackDirection;

                if (detachFromPlayer)
                {
                    // Use the attack's forward direction, but remove any vertical component.
                    knockbackDirection = transform.forward;
                    knockbackDirection.y = 0;
                    knockbackDirection = knockbackDirection.normalized;
                }
                else
                {
                    // Knockback away from the center of the attack's hitbox.
                    knockbackDirection = (other.transform.position - transform.position);
                    knockbackDirection.y = 0;  // Remove vertical momentum.
                    knockbackDirection = knockbackDirection.normalized;
                }

                enemyRb.AddForce(knockbackDirection * knockbackStrength, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
    }
}
