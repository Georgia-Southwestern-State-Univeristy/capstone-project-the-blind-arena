using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 targetPosition;
    public float speed;

    private void Start() => targetPosition = FindFirstObjectByType<PlayerController>().transform.position;

    private void Update()
    {
        MoveProjectile();

        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)  // Small margin to stop slightly before hitting the target
        {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile() 
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        } 
        else 
        {
            Debug.Log($"Projectile Hit: {collision.gameObject.name}");
        }
    }

    private void HandlePlayerCollision(GameObject player)
    {
        Debug.Log("Projectile hit the player!");
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.Damage(10);
        }
        Destroy(gameObject);
    }
}
