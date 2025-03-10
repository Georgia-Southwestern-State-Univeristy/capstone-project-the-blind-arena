using System;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileAttack : MonoBehaviour
{

    private enum Element { Earth, Wind, Fire, Water, Lightning };
    private enum Movement { Aimed, Homing, AimedHoming, Wandering, Stationary };

    [SerializeField] private Rigidbody projectile;
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
    private int counter=0;
    private Vector3 targetTransform;
    private Vector3 movementVector;
    private float fixedHeight = 0.6f;
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
    }

    // Update is called once per frame
    void Update()
    {
        MoveProjectile(moveType);
        ApplySpecialEffect(elementType);
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
                    movementVector.z = movementVector.z * 1.8f;
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

    private void ApplySpecialEffect(Element ele)
    {
        switch (ele)
        {
            //Earth Effects (Breaking Projectiles, etc.)
            case Element.Earth:

                break;
            //Wind Effects (Knockbacks, etc.)
            case Element.Wind:
                if (lifespan <= 0)
                {

                }
                break;
            //Fire Attack (Fires Tiles, Damage Over Times, etc.)
            case Element.Fire:
                if (lifespan <= 0)
                {

                }
                break;
            //Water Attack (Soaking, Freezing, etc.)
            case Element.Water:
                if (lifespan <= 0)
                {
                    //Create Water Tiles
                }
                break;
            //Lightning Attack (Stuns, Chaining, etc.)
            case Element.Lightning:

                break;
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
        Debug.Log($"Projectile Hit: {collision.gameObject.name}");
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
