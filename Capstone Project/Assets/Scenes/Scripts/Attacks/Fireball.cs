using UnityEngine;

public class Fireball : MonoBehaviour, IAttackBehavior
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int damage = 3;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private Vector3 colliderSize = new Vector3(1f, 1f, 1f);

    private float timer;
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }
        boxCollider.isTrigger = true;
        boxCollider.size = colliderSize;

        timer = 0f;
    }

    private void Update()
    {
        // Move the fireball forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Track lifetime
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for collisions with valid targets
        if (other.CompareTag("Enemy"))
        {
            // Apply damage logic
            var health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject); // Destroy fireball on hit
        }
    }

    public void TriggerAttack()
    {
        // Reset timer and position if needed
        timer = 0f;
    }
}
