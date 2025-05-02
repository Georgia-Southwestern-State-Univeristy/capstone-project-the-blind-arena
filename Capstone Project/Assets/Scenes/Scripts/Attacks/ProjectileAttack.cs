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
    private enum Movement { Aimed, Homing, AimedHoming, Retracting, Wandering, Stationary };

    [SerializeField] private GameObject sprite;
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Collider projectileCollider;
    [SerializeField] public Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float lifespan;
    [SerializeField] private int damage;
    [SerializeField] private Facing spriteDirection;
    [SerializeField] private Element elementType;
    [SerializeField] private Movement moveType;
    [SerializeField] private bool isEffect;
    [SerializeField] private bool delayDamage;
    [SerializeField] private bool vacuums;
    [SerializeField] private bool rotates;
    [SerializeField] private bool leavesTrail;
    [SerializeField] private bool breaksOnContact;

    private float initalLifespan, initalSpeed, fixedHeight, mCount = 0;
    private Vector3 targetTransform, movementVector, initialVector;
    private bool inTrigger, skipStart=false, attackLock=false, damageLock=false, moveLock = false, slowDown =false, speedUp=false;

    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioClip attackSound;

    // Public Function to initalize custom values
    public void Init(Transform targ, Vector3 vector)
    {
        skipStart =true;
        target = targ;
        targetTransform = target.position;
        movementVector = (vector).normalized;
        movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
    }

    void Start()
    {
        // Skip static values if initialized from anothe script
        if (!skipStart)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
                targetTransform = target.position;
                movementVector = (targetTransform - transform.position).normalized;
                movementVector *= ((Math.Abs(movementVector.z) * 0.6f) + 1) * speed;
            }
            else
            {
                Debug.LogWarning("No PlayerController found in the scene.");
                Destroy(gameObject); // Optional: destroy the projectile if no target
                return;
            }
        }

        if (delayDamage)
        {
            // Add delay logic here if needed
        }

        initialVector = movementVector;
        projectileCollider = GetComponent<Collider>();
        initalLifespan = lifespan;
        initalSpeed = speed;
        fixedHeight = isEffect ? 0.5f : 0.6f;
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
            // Moves in a straight Line
            case Movement.Aimed:
                break;
            // After half a second it will home in on the target, slows down over time
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
            // Moves in a straight line for a bit then slows to a stop. Waits and then shoots at the targets new position
            case Movement.AimedHoming:
                if (initalLifespan-lifespan > 0.4 && initalLifespan-lifespan < 3)
                {
                    if (!slowDown)
                    {
                        slowDown = true;
                        StartCoroutine(ApplySlowdown());
                    }
                    movementVector = initialVector * (speed / initalSpeed);
                }
                else if (initalLifespan-lifespan>3 && mCount==0)
                {
                    speed = initalSpeed;
                    PlayAttackSound(1);
                    targetTransform =target.position;
                    movementVector = (targetTransform - transform.position).normalized;
                    movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
                    mCount++;
                }
                break;
            // Moves in a straight line for a bit then slows to a stop. Waits and then reverses its direction
            case Movement.Retracting:
                if (initalLifespan - lifespan > 0.4 && initalLifespan - lifespan < 3)
                {
                    if (!slowDown)
                    {
                        slowDown = true;
                        StartCoroutine(ApplySlowdown());
                    }
                    movementVector = initialVector * (speed / initalSpeed);
                }
                else if (initalLifespan - lifespan > 3 && mCount == 0)
                {
                    if (!speedUp)
                    {
                        speedUp = true;
                        StartCoroutine(ApplySpeedUp());
                    }
                    movementVector = -initialVector * (speed / initalSpeed);
                }
                    break;
            // Wanders around randomly. See HandleProjectileWander
            case Movement.Wandering:
                if (initalLifespan - lifespan > 1)
                {
                    mCount=UnityEngine.Random.Range(1, 10);
                    StartCoroutine(HandleProjectileWander((int)mCount));
                }
                break;
            // No movement
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
            if (moveType == Movement.Wandering)
            {
                StartCoroutine(PlaceEffectTiles(2f));
            }
            else
                StartCoroutine(PlaceEffectTiles(.15f));
        }
        if (vacuums)
        {
            StartCoroutine(SuccPlayer());
        }
    }

    // Rotates Projectile in the movement Direction
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
        vector.x = 45;
        projectile.transform.eulerAngles = vector;
    }

    private IEnumerator ApplySlowdown()
    {
        while (initalLifespan-lifespan < 2)
        {
            speed = speed*.6f;
            yield return new WaitForSeconds(0.1f);
        } 
        while (initalLifespan - lifespan <= 3)
        {
            speed = speed * .3f;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator ApplySpeedUp()
    {
        while (speed < initalSpeed)
        {
            if (speed < 0.1f)
            {
                speed = 0.1f;
            }
            speed = speed * 2f;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.1f);
    }

    private void ApplySpecialEffect(Element ele, GameObject player, bool hit)
    {
        switch (ele)
        {
            //Earth Effects 
            case Element.Earth:
                if (isEffect)
                {
                    if (player)
                        StartCoroutine(DamageOverTime(player, damage));
                }
                break;
            //Wind Effects (Knockback)
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
            //Fire Attack (Fires Tiles, Damage Over Time)
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

    // Pulls the player to the projectile when close enough
    private IEnumerator SuccPlayer()
    {
        PlayerController[] playerController = FindObjectsOfType<PlayerController>();
        while (lifespan>0)
        {
            foreach (PlayerController player in playerController){
                if (Vector3.Distance(transform.position, player.transform.position) < 10)
                {
                    Vector3 pushDirection = (player.transform.position - transform.position).normalized;
                    Vector3 pushVelocity = -pushDirection * 7;
                    player.ApplyExternalForce(pushVelocity, 0.1f);
                }
                yield return new WaitForSeconds(.01f);
            }
        }
        yield return new WaitForSeconds(1f);
    }

    private void UpdateLifespan()
    {
        if (lifespan <= 0)
        {
            ApplySpecialEffect(elementType, null, false);
            StopAllCoroutines();

            Destroy(gameObject);
        }
        else if (delayDamage && initalLifespan - lifespan < 1)
        {
            projectileCollider.enabled = false;
        }
        else
        {
            projectileCollider.enabled = true;
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
        else if (collision.CompareTag("Wall"))
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
        if (!moveLock && lifespan>3)
        {
            moveLock = true;
            int pass=3;
            Debug.Log("Magnitude: " + magnitude);
            if (magnitude == 1 || magnitude==10)
            {
                while (pass > 0) //Change Dircetion when less than Zero
                {
                    //Move towards target
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
                        //Move Right
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
                        //Move Up
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
                        //Move Left
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
                        //Move Down
                        movementVector = (-transform.forward + movementVector.normalized).normalized;
                        movementVector *= ((Math.Abs(movementVector.z) * .6f) + 1) * speed;
                        pass--;
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            moveLock=false;
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

        // Cache the components outside the loop to avoid calling GetComponent repeatedly
        Health playerHealth = player.GetComponent<Health>();
        Animator animator = player.GetComponent<Animator>();

        // Check if the player or its components were destroyed early
        if (playerHealth == null || animator == null)
        {
            yield break;
        }

        if (elementType == Element.Earth)
        {
            if (inTrigger && !damageLock && initalLifespan - lifespan < 1)
            {
                damageLock = true;
                if (playerHealth != null)
                {
                    playerHealth.Damage(damage);
                    Debug.Log("Player is standing in a damage zone!");
                    yield return new WaitForSeconds(0.2f);
                }
            }
            else
            {
                while (inTrigger)
                {
                    if (animator.GetFloat("Speed") > 0f)
                    {
                        if (playerHealth != null)
                        {
                            playerHealth.Damage(damage / 10);
                            Debug.Log("Player moved on dangerous terrain!");
                        }
                    }
                    yield return new WaitForSeconds(1f);
                }
            }
        }
        else
        {
            while (inTrigger)
            {
                if (playerHealth != null)
                {
                    playerHealth.Damage(damage);
                    Debug.Log("Player is standing in a damage zone!");
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void PlayAttackSound(int soundIndex)
    {
        if (attackSound != null)
        {
            attackAudioSource.PlayOneShot(attackSound);
        }
        else
        {
            Debug.LogWarning($"Attack sound at index {soundIndex} is not assigned!");
        }
    }

}
