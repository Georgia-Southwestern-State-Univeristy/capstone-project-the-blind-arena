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

            Destroy(gameObject);
        }
    }
}