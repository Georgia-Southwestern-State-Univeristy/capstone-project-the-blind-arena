using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoss : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float speed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float minimumDistance = 5f;
    public float retreatSpeed = 10f;

    [Header("Attack Parameters")]
    public float attackDelay = 2f;
    public float lightningAttackRate = 1.5f;
    public float lightningProjectileSpeed = 8f;
    public float lightningProjectileLife = 5f;

    [Header("References")]
    public Transform target;
    public Transform returnWaypoint;
    public Animator animator;
    public GameObject[] lightningAttackPrefabs;
    public GameObject lightningDamageZonePrefab;

    [Header("Boss State")]
    private EnemyHealth enemyHealth;
    private bool isDashing;
    private bool isReturning;
    private bool isThrowingProjectiles;
    private bool isFinalPhase;
    private bool isKnockedBack = false;
    private float knockbackRecoveryTime = 0.5f;

    private float fixedHeight = 0.6f;
    private Vector3 retreatDirection;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            Debug.LogError("Rigidbody component not found!");
        }

        if (!enemyHealth) Debug.LogError("EnemyHealth component not found!");
    }

    private void Update()
    {
        if (!target)
        {
            Debug.LogWarning("No target assigned to LightningBoss!");
            return;
        }

        if (isKnockedBack)
        {
            return;
        }

        // Final phase (25% health)
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.25f)
        {
            if (!isFinalPhase)
            {
                isFinalPhase = true;
                isReturning = false;
                isDashing = false;
                isThrowingProjectiles = false;
                StopAllCoroutines();
                StartCoroutine(FinalPhaseRoutine());
            }
            return;
        }

        // Retreat phase (50% health)
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.5f &&
            enemyHealth.currentHealth > enemyHealth.maxHealth * 0.25f)
        {
            if (!isReturning)
            {
                isReturning = true;
                isDashing = false;
                isThrowingProjectiles = false;
                StopAllCoroutines();
                StartCoroutine(RetreatPhaseRoutine());
            }
            return;
        }

        // Normal phase
        if (!isDashing && !isReturning)
        {
            // Start lightning attacks if not already
            if (!isThrowingProjectiles)
            {
                StartCoroutine(LightningProjectileAttackLoop());
            }

            // Handle movement and dash attack
            if (Vector3.Distance(transform.position, target.position) <= minimumDistance)
            {
                if (!isDashing)
                {
                    StartCoroutine(LightningDashAttack());
                }
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - target.position.magnitude));
        FlipSprite(direction.x);

        Vector3 position = transform.position;
        position.y = fixedHeight;
        transform.position = position;
    }

    private IEnumerator LightningDashAttack()
    {
        isDashing = true;

        yield return new WaitForSeconds(0.5f);

        Vector3 dashDirection = (target.position - transform.position).normalized;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            transform.position += dashDirection * dashSpeed * Time.deltaTime;
            FlipSprite(dashDirection.x);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Spawn damage zone at dash end point
        SpawnLightningDamageZone();

        yield return new WaitForSeconds(0.5f);

        isDashing = false;
    }

    private IEnumerator LightningProjectileAttackLoop()
    {
        isThrowingProjectiles = true;
        while (isThrowingProjectiles)
        {
            if (lightningAttackPrefabs.Length > 0)
            {
                GameObject projectile = Instantiate(lightningAttackPrefabs[0], transform.position, Quaternion.identity);
                Vector3 direction = (target.position - transform.position).normalized;
                FlipSprite(target.position.x);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * lightningProjectileSpeed;
                }
            }

            yield return new WaitForSeconds(lightningAttackRate);
        }
    }

    private IEnumerator RetreatPhaseRoutine()
    {
        while (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.5f &&
               enemyHealth.currentHealth > enemyHealth.maxHealth * 0.25f &&
               isReturning)
        {
            Vector3 directionToWaypoint = (returnWaypoint.position - transform.position).normalized;
            FlipSprite(directionToWaypoint.x);

            transform.position += directionToWaypoint * retreatSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, returnWaypoint.position) < 0.5f)
            {
                ShootLightningProjectilesInArc(8, 120f);
                FlipSprite(target.position.x);

                yield return new WaitForSeconds(2f);

                Vector3 randomOffset = Random.insideUnitSphere * 3f;
                randomOffset.y = 0;
                Vector3 newPosition = returnWaypoint.position + randomOffset;

                Vector3 position = transform.position;
                position.y = fixedHeight;
                transform.position = position;

                while (Vector3.Distance(transform.position, newPosition) > 0.5f &&
                       isReturning &&
                       enemyHealth.currentHealth > enemyHealth.maxHealth * 0.25f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, newPosition, retreatSpeed * Time.deltaTime);
                    yield return null;
                }
            }

            yield return null;
        }
    }

    private IEnumerator FinalPhaseRoutine()
    {
        while (true)
        {
            retreatDirection = (transform.position - target.position).normalized;

            GameObject projectile = Instantiate(lightningAttackPrefabs[0], transform.position, Quaternion.identity);
            Vector3 directionToPlayer = (target.position - transform.position).normalized;
            FlipSprite(directionToPlayer.x);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = directionToPlayer * lightningProjectileSpeed;
            }

            Vector3 targetPosition = transform.position + (retreatDirection * 10f);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, retreatSpeed * Time.deltaTime);

            if (Physics.Raycast(transform.position, retreatDirection, 2f))
            {
                Vector3 sideDirection = Vector3.Cross(retreatDirection, Vector3.up).normalized;
                sideDirection = Random.value > 0.5f ? -sideDirection : sideDirection;

                targetPosition = transform.position + (sideDirection * 5f);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, retreatSpeed * Time.deltaTime);
            }

            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer < minimumDistance * 2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, retreatSpeed * 1.5f * Time.deltaTime);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void OnKnockback()
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            isDashing = false;
            isThrowingProjectiles = false;

            StopAllCoroutines();
            StartCoroutine(KnockbackRecovery());
        }
    }

    private IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(knockbackRecoveryTime);
        isKnockedBack = false;

        if (isFinalPhase)
        {
            StartCoroutine(FinalPhaseRoutine());
        }
        else if (isReturning)
        {
            StartCoroutine(RetreatPhaseRoutine());
        }
        else
        {
            StartCoroutine(LightningProjectileAttackLoop());
        }
    }

    private void FlipSprite(float directionX)
    {
        transform.localScale = new Vector3(Mathf.Sign(directionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void SpawnLightningDamageZone()
    {
        if (lightningDamageZonePrefab)
        {
            Instantiate(lightningDamageZonePrefab, transform.position, Quaternion.identity);
        }
    }

    private void ShootLightningProjectilesInCircle(int projectileCount)
    {
        float angleStep = 360f / projectileCount;
        float angle = 0f;

        for (int i = 0; i < projectileCount; i++)
        {
            float projectileDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 projectileDirection = new Vector3(projectileDirX, projectileDirY, 0f).normalized;

            GameObject projectile = Instantiate(lightningAttackPrefabs[0], transform.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = projectileDirection * lightningProjectileSpeed;
            }

            angle += angleStep;
        }
    }

    private void ShootLightningProjectilesInArc(int projectileCount, float arcAngle)
    {
        float startAngle = -arcAngle / 2f;
        float angleStep = arcAngle / (projectileCount - 1);

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            float projectileDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 projectileDirection = new Vector3(projectileDirX, projectileDirY, 0f).normalized;

            GameObject projectile = Instantiate(lightningAttackPrefabs[0], transform.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = projectileDirection * lightningProjectileSpeed;
            }
        }
    }
}