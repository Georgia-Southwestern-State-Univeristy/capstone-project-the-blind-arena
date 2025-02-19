using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damagePerSecond = 10; // How much damage per second
    public int damageAmount;
    public float duration = 5f; // How long the zone exists
    private HashSet<Health> affectedPlayers = new HashSet<Health>(); // Track players inside

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
        StartCoroutine(ApplyDamageOverTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private IEnumerator ApplyDamageOverTime()
    {
        while (true)
        {
            foreach (Health player in affectedPlayers)
            {
                if (player != null)
                {
                    player.Damage(damagePerSecond);
                }
            }
            yield return new WaitForSeconds(1f); // Damage applies every second
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        {
            Debug.Log("Player Detected in Attack");
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(damageAmount);
            }
        }
    }
}

