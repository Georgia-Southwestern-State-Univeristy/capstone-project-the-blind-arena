using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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

    private float initalLifespan, initalSpeed, fixedHeight, mCount=0, tCount=0;
    private Vector3 targetTransform;
    private Vector3 movementVector;
    private bool inTrigger, takingDamage, attackLock=false;
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
                if (initalLifespan - lifespan > 1 && lifespan >= 4)
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
                        speed = speed * 0.9f;
                        movementVector = movementVector * (speed / initalSpeed);
                    }
                }
                else if (initalLifespan-lifespan>3 && mCount==0)
                {
                    speed = initalSpeed;
                    targetTransform=target.position;
                    movementVector = (targetTransform - transform.position).normalized * speed;
                    movementVector *= (Math.Abs(movementVector.z)/2)+1;
                    mCount++;
                }
                break;
            case Movement.Wandering:
                if (initalLifespan - lifespan > 1)
                {
                    mCount=UnityEngine.Random.Range(1, 10);
                    StartCoroutine(HandleProjectileWander((int)mCount));
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

    private void ApplySpecialEffect(Element ele, GameObject player)
    {
        switch (ele)
        {
            //Earth Effects (Breaking Projectiles, etc.)
            case Element.Earth:

                break;
            //Wind Effects (Knockbacks, etc.)
            case Element.Wind:
                if (!isEffect)
                {
                    if (player != null)
                    {
                        PlayerController playerController = player.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            Vector3 pushDirection = (player.transform.position - transform.position).normalized;
                            Vector3 pushVelocity = pushDirection * 15;
                            playerController.ApplyExternalForce(pushVelocity, 0.2f);
                        }
                    }
                }
                else
                {

                }
                                
                break;
            //Fire Attack (Fires Tiles, Damage Over Times, etc.)
            case Element.Fire:
                takingDamage=true;
                if (!isEffect) 
                {
                    StartCoroutine(PlaceEffectTiles());
                }
                else if (inTrigger)
                {
                    if (player != null)
                        StartCoroutine(DamageOverTime(player, damage));
                }
                if (takingDamage)
                {
                    if (player != null)
                        StartCoroutine(DamageOverTime(player, 1));
                }
                break;
            //Water Attack (Place Water, Freezing, etc.)
            case Element.Water:
                if(!isEffect)
                {
                    StartCoroutine(PlaceEffectTiles());
                }
                else
                {

                }
                break;
            //Lightning Attack (Stuns, Chaining, etc.)
            case Element.Lightning:
                if (!isEffect)
                {
                    if (player != null)
                    {
                        PlayerController playerController = player.GetComponent<PlayerController>();
                        if (playerController != null)
                        {
                            playerController.LockMovement(1);
                            Debug.Log("Lightning Stun");
                        }
                    }
                }
                else 
                {
                    
                }
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
        if (collision.tag == "Player")
        {
            if (isEffect)
            {
                inTrigger = true;
                ApplySpecialEffect(elementType, collision.gameObject);
            }
            else
            {
                HandlePlayerCollision(collision.gameObject);
                ApplySpecialEffect(elementType, collision.gameObject);
            }
        }
    }
    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag=="Player")
        {
            if (breaksOnContact)
            {
                lifespan = 0;
            }
            else
                lifespan -= Time.deltaTime*2;
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
    private void OnTriggerExit(Collider other)
    {
        inTrigger=false;
        ApplySpecialEffect(elementType, other.gameObject);
    }

    private IEnumerator HandleProjectileWander(int magnitude)
    {
        if (!attackLock && lifespan>3)
        {
            attackLock = true;
            int pass=3;
            if (magnitude == 1 || magnitude==10)
            {
                while (pass > 0)
                {
                    targetTransform = target.position;
                    movementVector = (targetTransform - transform.position).normalized * speed;
                    pass--;
                    Debug.Log("Magnitude: " + magnitude);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else if (magnitude % 2 == 0)
            {
                if (magnitude > 5)
                {
                    while (pass > 0)
                    {
                        movementVector = (transform.right + movementVector.normalized).normalized * speed;
                        movementVector.z = movementVector.z * 1.8f;
                        pass--;
                        Debug.Log("Magnitude: " + magnitude);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else
                {
                    while (pass > 0)
                    {
                        movementVector = (transform.forward + movementVector.normalized).normalized * speed;
                        movementVector.z = movementVector.z * 1.8f;
                        pass--;
                        Debug.Log("Magnitude: " + magnitude);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            else
            {
                if (magnitude > 5)
                {
                    while (pass > 0)
                    {
                        movementVector = (-transform.right+movementVector.normalized).normalized * speed;
                        movementVector.z = movementVector.z * 1.8f;
                        pass--;
                        Debug.Log("Magnitude: " + magnitude);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else
                {
                    while (pass > 0)
                    {
                        movementVector = (-transform.forward + movementVector.normalized).normalized * speed;
                        movementVector.z = movementVector.z * 1.8f;
                        pass--;
                        Debug.Log("Magnitude: " + magnitude);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            attackLock=false;
        }
    }

    private void HandlePlayerCollision(GameObject player)
    {
        Debug.Log("Projectile hit the player!");
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.Damage(damage);
            ApplySpecialEffect(elementType, player);
        }
    }

    private IEnumerator PlaceEffectTiles()
    {
        if (!attackLock)
        {
            attackLock = true;
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            attackLock = false;
        }
    }

    public IEnumerator DamageOverTime(GameObject player, int damage)
    {
        tCount = 0;
        while (inTrigger)
        {
            Health playerHealth = player.GetComponent<Health>();
            playerHealth.Damage(damage);
            Debug.Log("Player is standing in a damage zone!");
            yield return new WaitForSeconds(0.5f);
        }
        while (takingDamage)
        {
            if (tCount >= 5)
            {
                takingDamage =false;
            }
            Health playerHealth = player.GetComponent<Health>();
            playerHealth.Damage(damage);
            Debug.Log("Player took damage over time!");
            tCount++;
            yield return new WaitForSeconds(1f);
        }
    }
}
