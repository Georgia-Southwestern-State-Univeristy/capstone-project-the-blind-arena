using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileAttack : MonoBehaviour
{
    public Rigidbody projectile;
    public float speed;
    public float lifespan;
    public int damage;
    public Rigidbody target;

    public bool isHoming;
    public bool isAimed;
    public bool isWandering;
    public bool isStationary;
    public bool isTemporary;
    public int type;

    private float initalLifespan;
    private Transform targetTransform;
    private Vector3 movementVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = FindFirstObjectByType<PlayerController>().;
        targetTransform = FindFirstObjectByType<PlayerController>().transform;
        movementVector = (targetTransform.position - transform.position).normalized * speed;
        projectile = GetComponent<Rigidbody>();
        initalLifespan = lifespan;
    }

    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
        UpdateLifespan();

    }

    private void MoveProjectile()
    {
        if (isHoming) 
        {
            if (initalLifespan-lifespan <= 2)
            {
                transform.position += movementVector * Time.deltaTime;
            }
            else
            {
                targetTransform = FindFirstObjectByType<PlayerController>().transform;
                transform.position += movementVector * Time.deltaTime;
            }
        }
        if (isAimed)
        {
            transform.position += movementVector * Time.deltaTime;
        }
        
    }

    private void UpdateLifespan()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void SpecialEffect()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
        else
        if (isTemporary) 
        { 
            if (collision.gameObject.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
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
            playerHealth.Damage(damage);
        }
        Destroy(gameObject);
    }
}
