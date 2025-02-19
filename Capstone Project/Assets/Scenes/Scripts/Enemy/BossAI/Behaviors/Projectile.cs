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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Projectile hit the player!");
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Damage(10);
            }
            Destroy(gameObject);
        } 
        else 
        {
            Debug.Log("Projectile Hit: " + collision.gameObject.name);
        }
    }
}
