using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ProjectileAttack : MonoBehaviour
{

    private enum Facing { Up, Down, Right, Left };
    private enum Element { Earth, Wind, Fire, Water, Lightning };
    private enum Movement { Aimed, Homing, AimedHoming, Wandering, Stationary };

    [SerializeField] private GameObject sprite;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] public Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float lifespan;
    [SerializeField] private int damage;
    [SerializeField] private Facing spriteDirection;
    [SerializeField] private Element elementType;
    [SerializeField] private Movement moveType;
    [SerializeField] private bool isEffect;
    [SerializeField] private bool rotates;
    [SerializeField] private bool leavesTrail;
    [SerializeField] private bool breaksOnContact;

    private float initalLifespan, initalSpeed, fixedHeight, mCount=0, tCount=0;
    private Vector3 targetTransform, movementVector;
    private bool inTrigger, takingDamage, skipStart=false, attackLock=false, damageLock=false;

    public void Init(Transform targ, Vector3 vector)
    {
        skipStart =true;
        target = targ;
        targetTransform = target.position;
        movementVector = (vector).normalized;
        movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!skipStart)
        {
            target = FindFirstObjectByType<PlayerController>().transform;
            targetTransform = target.position;
            movementVector = (targetTransform - transform.position).normalized;
            movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
        }
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
                if (initalLifespan - lifespan > 0.5 && lifespan >= 3)
                {
                    targetTransform = target.position;

                    Vector3 aimVector = targetTransform - transform.position;
                    Vector3 forward = transform.forward;
                    forward = Vector3.Slerp(forward, aimVector, (speed/3)*Time.deltaTime);
                    transform.forward = forward;
                    if (lifespan / initalLifespan > 0.5)
                    {
                        movementVector = transform.forward * (lifespan / initalLifespan);
                        movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
                    }
                        
                    else
                    {
                        movementVector = transform.forward;
                        movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * (speed/2);
                    }
                        
                }
                break;
            case Movement.AimedHoming:
                if (initalLifespan-lifespan > 0.5 && initalLifespan-lifespan < 3)
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
                    movementVector = (targetTransform - transform.position).normalized;
                    movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
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
        
        if (rotates)
        {
            ApplyRotation(sprite, spriteDirection, movementVector, speed);
        }
        if (leavesTrail && effectPrefab!=null)
        {
            StartCoroutine(PlaceEffectTiles(.15f));
        }
    }

    private void ApplyRotation(GameObject projectile, Facing direction, Vector3 vector, float rSpeed)
    {
        float spin=0;
        if (direction == Facing.Up)
        {
            spin = 180;
        }
        else if (direction == Facing.Right) 
        {
            spin = 90;
        }
        else if (direction == Facing.Left)
        {
            spin = -90;
        }
        vector = vector.normalized;
        if (vector.z > 0)
        {
            vector.z = -vector.x;
            vector = vector * 90;
            vector.z += spin;
        }
        else
        {
            vector.z = vector.x;
            vector = vector * 90;
            vector.z -= spin;
        }
        vector.y = 0;
        vector.x = 0;
        projectile.transform.eulerAngles = vector;
    }

    private void ApplySpecialEffect(Element ele, GameObject player, bool hit)
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
                if (!isEffect && lifespan<=0) 
                {
                    StartCoroutine(PlaceEffectTiles(1f));
                }
                else if (inTrigger)
                {
                    if (player)
                        StartCoroutine(DamageOverTime(player, damage));
                }
                break;
            //Water Attack (Place Water, Freezing, etc.)
            case Element.Water:
                if(!isEffect)
                {
                    StartCoroutine(PlaceEffectTiles(1f));
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
            ApplySpecialEffect(elementType, null, false);
            StopAllCoroutines();
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
        if (collision.CompareTag("Player"))
        {
            if (isEffect)
            {
                inTrigger = true;
                ApplySpecialEffect(elementType, collision.gameObject, false);
            }
            else
            {
                HandlePlayerCollision(collision.gameObject);
                ApplySpecialEffect(elementType, collision.gameObject, false);
            }
        }
    }
    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (breaksOnContact)
            {
                lifespan = 0;
            }
            else
                lifespan -= Time.deltaTime*2;
        }
        else
        if (collision.CompareTag("Wall"))
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
        ApplySpecialEffect(elementType, other.gameObject, true);
    }

    private IEnumerator HandleProjectileWander(int magnitude)
    {
        if (!attackLock && lifespan>3)
        {
            attackLock = true;
            int pass=3;
            Debug.Log("Magnitude: " + magnitude);
            if (magnitude == 1 || magnitude==10)
            {
                while (pass > 0)
                {
                    targetTransform = target.position;
                    movementVector = (targetTransform - transform.position).normalized;
                    movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
                    pass--;
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else if (magnitude % 2 == 0)
            {
                if (magnitude > 5)
                {
                    while (pass > 0)
                    {
                        movementVector = (transform.right + movementVector.normalized).normalized;
                        movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
                        pass--;
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else
                {
                    while (pass > 0)
                    {
                        movementVector = (transform.forward + movementVector.normalized).normalized;
                        movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
                        pass--;
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
                        movementVector = (-transform.right+movementVector.normalized).normalized;
                        movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
                        pass--;
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else
                {
                    while (pass > 0)
                    {
                        movementVector = (-transform.forward + movementVector.normalized).normalized;
                        movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
                        pass--;
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
            ApplySpecialEffect(elementType, player, true);
        }
    }

    private IEnumerator PlaceEffectTiles(float interval)
    {
        if (!attackLock)
        {
            attackLock = true;
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(interval);
            attackLock = false;
        }
    }

    public IEnumerator DamageOverTime(GameObject player, int damage)
    {
        if (this == null || player == null)
        {
            yield break;
        }
        while (inTrigger)
        {
            Health playerHealth = player.GetComponent<Health>();
            playerHealth.Damage(damage);
            Debug.Log("Player is standing in a damage zone!");
            yield return new WaitForSeconds(0.5f);
        }
    }
    
}
