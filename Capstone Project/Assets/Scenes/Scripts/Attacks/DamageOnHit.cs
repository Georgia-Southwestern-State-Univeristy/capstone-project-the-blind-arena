using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    public int damageAmount;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Triggered with {other.name}");

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.Damage(damageAmount);
            Debug.Log($"Dealt {damageAmount} damage to {other.name}");
            Destroy(gameObject);
        }
    }
}
