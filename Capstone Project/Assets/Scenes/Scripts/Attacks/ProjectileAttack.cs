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

    public bool isHoming;
    public bool isAimed;
    public bool isWandering;
    public bool isStationary;
    public bool breaksOnContact;
    public bool rotates;
    public int type;

    private float initalLifespan;
    public Transform target;
    private Transform targetTransform;
    private Vector3 movementVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = FindFirstObjectByType<PlayerController>().transform;
        targetTransform = FindFirstObjectByType<PlayerController>().transform;
        movementVector = (targetTransform.position - transform.position).normalized * speed;
        projectile = GetComponent<Rigidbody>();
        initalLifespan = lifespan;
    }

    // Update is called once per frame
    void Update()
    {
        MoveProjectile();
        ApplyRotation();
        ApplySpecialEffect();
        UpdateLifespan();
    }

    private void MoveProjectile()
    {
        if (isHoming) 
        {
            if (initalLifespan-lifespan <= 1 || lifespan/initalLifespan <=0.2)
            {
                transform.position += movementVector * Time.deltaTime;
            }
            else
            {
                targetTransform = target.transform;
                movementVector = (targetTransform.position - transform.position).normalized * speed * ((lifespan / initalLifespan));
                transform.position += movementVector * Time.deltaTime;
            }
        }
        if (isAimed)
        {
            transform.position += movementVector * Time.deltaTime;
        }
        if (isWandering)
        {
            movementVector *= Time.deltaTime;
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

    private void ApplySpecialEffect()
    {
        switch (type)
        {
            //Earth Attacks
            case 1:

                break;
            //Wind Attack
            case 2:

                break;
            //Fire Attack
            case 3:

                break;
            //Water Attack
            case 4:

                break;
            //Lightning Attack
            case 5:

                break;
        }
    }

    private void ApplyRotation()
    {
        /*
        projectile.constraints = RigidbodyConstraints.FreezeRotationX;
        projectile.constraints = RigidbodyConstraints.FreezeRotationY;
        if (!rotates)
        {
            projectile.constraints = RigidbodyConstraints.FreezeRotationZ;
        }
        else 
        {
            Quaternion wantedRotation = Quaternion.Euler(movementVector);
            projectile.rotation = Quaternion.Slerp(projectile.rotation, wantedRotation, 1);
        }
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
        else
        if (breaksOnContact) 
        { 
            if (collision.gameObject.CompareTag("Wall"))
            {
                lifespan = 0;
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
        lifespan = 0;
    }
}
