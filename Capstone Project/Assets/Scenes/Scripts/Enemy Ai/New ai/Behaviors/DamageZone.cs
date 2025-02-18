using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public float damagePerSecond = 10f; // How much damage per second
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
                    player.TakeDamage(damagePerSecond);
                }
            }
            yield return new WaitForSeconds(1f); // Damage applies every second
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure the player has this tag
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                affectedPlayers.Add(playerHealth);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                affectedPlayers.Remove(playerHealth);
            }
        }
    }
}

