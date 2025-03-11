using System;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{

    private enum Element { Earth, Wind, Fire, Water, Lightning };
    private enum Movement { Aimed, Homing, AimedHoming, Wandering, Stationary };

    [SerializeField] private Rigidbody projectile;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float lifespan;
    [SerializeField] private int damage;
    [SerializeField] private Element elementType;
    [SerializeField] private Movement moveType;
    [SerializeField] private bool isEffect;
    [SerializeField] private bool breaksOnContact;

    private float initalLifespan;
    private float initalSpeed;
    private float counter=0;
    private Vector3 targetTransform;
    private Vector3 movementVector;
    private float fixedHeight;
    private bool wasHit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = FindFirstObjectByType<PlayerController>().transform;
        targetTransform = target.position;
        movementVector = (targetTransform - transform.position).normalized * speed;
        movementVector.z = movementVector.z * 1.8f;
        projectile = GetComponent<Rigidbody>();
        initalLifespan = lifespan;
        initalSpeed = speed;
        if (isEffect) { fixedHeight = 0.5f; } else { fixedHeight = 0.6f; }
    }

    // Update is called once per frame
    void Update()
    {
        MoveProjectile(moveType);
        UpdateLifespan();
    }

    private void MoveProjectile(Movement mov)
    {
        switch (mov)
        {
            case Movement.Aimed:
                break;
            case Movement.Homing:
                if (initalLifespan - lifespan > 1 || lifespan / initalLifespan > 0.2)
                {
                    targetTransform = target.position;
                    movementVector = (targetTransform - transform.position).normalized * speed * (lifespan / initalLifespan);
                    movementVector.z = movementVector.z * 1.8f;
                }
                break;
            case Movement.AimedHoming:
                if (initalLifespan-lifespan > 1 && initalLifespan-lifespan < 3)
                {
                    if (speed >= 0)
                    {
                        speed = speed * 0.5f;
                        movementVector = movementVector * (speed / initalSpeed);
                    }
                }
                else if (initalLifespan-lifespan>3 && counter==0)
                {
                    speed = initalSpeed;
                    targetTransform=target.position;
                    movementVector = (targetTransform - transform.position).normalized * speed;
                    movementVector *= (Math.Abs(movementVector.z)/2)+1;
                    counter++;
                }
                break;
            case Movement.Wandering:
                if (initalLifespan - lifespan > 1 || lifespan / initalLifespan > 0.2)
                {
                }
                break;
            case Movement.Stationary:
                movementVector = Vector3.zero;
                break;
        }
        transform.position += movementVector * Time.deltaTime;
        Vector3 position = transform.position;
        position.y = fixedHeight;
        transform.position = position;
    }

    private void ApplySpecialEffect(Element ele, GameObject target)
    {
        switch (ele)
        {
            //Earth Effects (Breaking Projectiles, etc.)
            case Element.Earth:

                break;
            //Wind Effects (Knockbacks, etc.)
            case Element.Wind:

                break;
            //Fire Attack (Fires Tiles, Damage Over Times, etc.)
            case Element.Fire:
                if (!isEffect) 
                {
                    Instantiate(effectPrefab, transform.position, Quaternion.identity); 
                }
                if (wasHit && target!=null)
                {
                    int burnTime;
                    Health targethealth= target.GetComponent<Health>();
                    if (targethealth != null)
                    {
                        
                    }
                }
                break;
            //Water Attack (Pace Water, Freezing, etc.)
            case Element.Water:
                if(!isEffect)
                {
                    Instantiate(effectPrefab, transform.position, Quaternion.identity);
                }
                else
                {

                }
                break;
            //Lightning Attack (Stuns, Chaining, etc.)
            case Element.Lightning:

                break;
        }
    }

    private void UpdateLifespan()
    {
        if (lifespan <= 0)
        {
            ApplySpecialEffect(elementType, null);
            Destroy(gameObject);
        }
        lifespan -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
            if (breaksOnContact)
            {
                lifespan = 0;
            }
        }
        else
        if (collision.gameObject.CompareTag("Wall")) 
        { 
            if (breaksOnContact)
            {
                lifespan = 0;
                Debug.Log($"Projectile Hit: {collision.gameObject.name}");
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        counter=0;
    }
    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag=="Player")
        {
            if (counter >= 1)
            {
                counter = 0;
            }
            if (counter == 0)
            {
                HandlePlayerCollision(collision.gameObject);
            }
            counter += Time.deltaTime;
            if (breaksOnContact)
            {
                lifespan = 0;
            }
            else
                lifespan -= Time.deltaTime;
        }
        else
        if (collision.tag == "Wall")
        {
            if (breaksOnContact)
            {
                lifespan = 0;
            }
        }
    }


    private void HandlePlayerCollision(GameObject player)
    {
        Debug.Log("Projectile hit the player!");
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.Damage(damage);
            wasHit = true;
            ApplySpecialEffect(elementType, player);
        }
    }
}
