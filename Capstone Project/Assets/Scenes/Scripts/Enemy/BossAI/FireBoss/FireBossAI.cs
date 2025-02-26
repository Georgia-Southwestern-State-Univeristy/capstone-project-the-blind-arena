using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBossAI : MonoBehaviour
{
    public float speed;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public Animator animator;
    public GameObject[] attackPrefabs;
    public GameObject damageZonePrefab;

    private EnemyHealth enemyHealth;
    private ShootAndRetreat shootAndRetreat;
    private bool isDashing;
    private bool isReturning;
    private bool hasUsedSecondAbility;
    private bool hasEnteredShootAndRetreat;
    private bool isThrowingProjectiles;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        shootAndRetreat = GetComponent<ShootAndRetreat>();

        if (!enemyHealth) Debug.LogError("EnemyHealth component not found!");
        if (!shootAndRetreat) Debug.LogError("ShootAndRetreat component not found!");
    }

    private void Update()
    {
        if (hasEnteredShootAndRetreat) 
        {
            shootAndRetreat.enabled = true;
            return;
        }

        if (isReturning)
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
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - target.position.magnitude));
        FlipSprite(direction.x);
    }

    private IEnumerator DashAttack()
    {
        isDashing = true;
        isThrowingProjectiles = false;

        yield return new WaitForSeconds(0.5f); // Wind-up before dashing

        Vector3 dashDirection = (target.position - transform.position).normalized;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            transform.position += dashDirection * dashSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // Pause after dash

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
                Instantiate(attackPrefabs[Random.Range(0, attackPrefabs.Length)], transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(projectileAttackRate);

            // If player is close enough, switch to dash
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
        animator?.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);

        if (attackPrefabs.Length > 1)
        {
            Instantiate(attackPrefabs[1], transform.position, Quaternion.identity);
        }

        isReturning = false;
    }

    private void FlipSprite(float directionX)
    {
        transform.localScale = new Vector3(Mathf.Sign(directionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void SpawnDamageZone()
    {
        if (damageZonePrefab)
        {
            Instantiate(damageZonePrefab, transform.position, Quaternion.identity);
        }
    }
}
