using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 targetInitialPos;
    public float speed;
    public float lifetime;

    private float counter=0;

    private void Start() 
    {
        targetInitialPos = FindFirstObjectByType<PlayerController>().transform.position;
    }

    private void Update()
    {
        MoveProjectile();
        counter+=0.01f;
        targetPosition = FindFirstObjectByType<PlayerController>().transform.position;


        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f || counter > lifetime)  // Small margin to stop slightly before hitting the target
        {
            Destroy(gameObject);
            counter = 0;
        }
    }

    private void MoveProjectile() 
    {
        transform.position = Vector3.MoveTowards(transform.position, targetInitialPos, speed * Time.deltaTime);
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
