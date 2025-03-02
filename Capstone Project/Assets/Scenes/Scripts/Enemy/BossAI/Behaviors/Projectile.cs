using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 targetPosition;
    public float speed;

    // Wind boss specific properties
    private bool isWindProjectile;
    private bool shouldFollowPlayer;
    private Transform playerTransform;
    private float windProjectileSpeed;
    private float windProjectileLifespan;
    private float timer;

    private void Start()
    {
        playerTransform = FindFirstObjectByType<PlayerController>().transform;
        targetPosition = playerTransform.position;
        timer = windProjectileLifespan;
    }

    private void Update()
    {
        if (isWindProjectile)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Destroy(gameObject);
                return;
            }

            if (shouldFollowPlayer)
            {
                // Continuously update target position for wind projectiles in follow mode
                targetPosition = playerTransform.position;
            }
        }

        MoveProjectile();

        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        float currentSpeed = isWindProjectile ? windProjectileSpeed : speed;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
    }

    // Method specifically for wind boss to enable following behavior
    public void EnableWindProjectile(bool followPlayer, float projectileSpeed, float lifespan)
    {
        isWindProjectile = true;
        shouldFollowPlayer = followPlayer;
        windProjectileSpeed = projectileSpeed;
        windProjectileLifespan = lifespan;
        timer = lifespan;
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
