using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector3 targetPosition;
    public float speed;

    private void Start()
    {
        targetPosition = FindFirstObjectByType<PlayerController>().transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Debug log to confirm collision detection
            Debug.Log("Projectile hit the player!");

            Health playerHealth = collision.gameObject.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // Adjust damage value as needed
            }

            Destroy(gameObject); // Destroy the projectile after collision
        }
        else
        {
            // Debug log for checking collision with non-player objects
            Debug.Log("Projectile hit: " + collision.gameObject.name);
        }
    }
}
