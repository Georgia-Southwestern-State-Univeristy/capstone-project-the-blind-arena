using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBossAI : MonoBehaviour
{
    public float speed;
    public float dashSpeed = 15f; // Speed for dash attack
    public float dashDuration = 0.2f; // Duration of dash attack
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f; // Rate at which projectiles are thrown
    public Animator animator;
    public GameObject[] attackPrefabs;
    public GameObject damageZonePrefab; // Assign in Unity Inspector

    private EnemyHealth enemyHealth;
    private Shootandretreat shootAndRetreat;
    private bool isDashing = false;
    private bool isReturning = false;
    private bool hasUsedSecondAbility = false;
    private bool hasEnteredShootAndRetreat = false;
    private bool isThrowingProjectiles = false;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        shootAndRetreat = GetComponent<Shootandretreat>();

        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth component not found!");
        }
        if (shootAndRetreat == null)
        {
            Debug.LogError("Shootandretreat component not found!");
        }
    }

    private void Update()
    {
        if (hasEnteredShootAndRetreat)
        {
            shootAndRetreat.enabled = true;
        }
        else if (isReturning)
        {
            ReturnToWaypoint();
        }
        else if (Vector3.Distance(transform.position, target.position) > minimumDistance && !isDashing)
        {
            MoveTowardsPlayer();
        }
        else if (!isDashing)
        {
            StartCoroutine(DashAttack());
        }

        if (enemyHealth.currentHealth <= enemyHealth.maxHealth / 2 && !isReturning)
        {
            isReturning = true;
        }

        if (enemyHealth.currentHealth <= enemyHealth.maxHealth / 4 && !hasEnteredShootAndRetreat)
        {
            hasEnteredShootAndRetreat = true;
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 movement = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.position = movement;
        animator.SetFloat("speed", movement.magnitude);
        FlipSprite(direction.x);
    }

    private IEnumerator DashAttack()
    {
        isDashing = true;
        isThrowingProjectiles = false;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(0.5f); // Short wind-up before dashing

        Vector3 dashDirection = (target.position - transform.position).normalized;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            transform.position += dashDirection * dashSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // Small pause after dashing

        isDashing = false;
        isThrowingProjectiles = true;
        StartCoroutine(ProjectileAttackLoop());
    }

    private IEnumerator ProjectileAttackLoop()
    {
        while (isThrowingProjectiles)
        {
            if (attackPrefabs.Length > 0)
            {
                int attackIndex = Random.Range(0, attackPrefabs.Length);
                Instantiate(attackPrefabs[attackIndex], transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(projectileAttackRate);

            // Stop throwing if the player is close enough for another dash
            if (Vector3.Distance(transform.position, target.position) <= minimumDistance)
            {
                isThrowingProjectiles = false;
                StartCoroutine(DashAttack());
                yield break;
            }
        }
    }

    private void ReturnToWaypoint()
    {
        if (Vector3.Distance(transform.position, returnWaypoint.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, returnWaypoint.position, speed * Time.deltaTime);
        }
        else if (!hasUsedSecondAbility)
        {
            StartCoroutine(UseSecondAbility());
        }
    }

    private IEnumerator UseSecondAbility()
    {
        hasUsedSecondAbility = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(attackDelay);

        if (attackPrefabs.Length > 1)
        {
            Instantiate(attackPrefabs[1], transform.position, Quaternion.identity);
        }

        isReturning = false;
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void SpawnDamageZone()
    {
        if (damageZonePrefab != null)
        {
            Instantiate(damageZonePrefab, transform.position, Quaternion.identity);
        }
    }
}