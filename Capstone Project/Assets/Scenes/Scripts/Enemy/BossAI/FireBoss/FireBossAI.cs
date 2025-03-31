using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBossAI : MonoBehaviour
{
    public float speed;
    public float dashSpeed = 15f;
    public float dashDuration = 0.5f;
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance = 5f;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public float projectileSpeed = 8f;
    public float projectileLife = 5f;
    public float retreatSpeed = 10f;
    public Animator animator;
    public GameObject[] attackPrefabs;
    public GameObject damageZonePrefab;

    private EnemyHealth enemyHealth;
    private bool isDashing;
    private bool isReturning;
    private bool hasUsedSecondAbility;
    private bool isThrowingProjectiles;
    private bool isFinalPhase;
    private Vector3 retreatDirection;
    private bool isKnockedBack = false;
    private bool targetLock = false;
    private float knockbackRecoveryTime = 0.5f;

    private float fixedHeight = 0.6f;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;  // Prevent the boss from rotating when hit
            rb.constraints = RigidbodyConstraints.FreezeRotation;  // Only freeze rotation, allow position changes
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
        StartCoroutine(CheckForTarget());
        }

        // If being knocked back, don't run normal movement logic
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
                isReturning = false;  // Stop the return to waypoint behavior
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
            // Start shooting if not already
            if (!isThrowingProjectiles)
            {
                StartCoroutine(ProjectileAttackLoop());
            }

            // Handle movement
            if (Vector3.Distance(transform.position, target.position) <= minimumDistance)
            {
                if (!isDashing)
                {
                    StartCoroutine(DashAttack());
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

    private IEnumerator CheckForTarget()
    {
        if (!targetLock)
        {
            targetLock = true;
            target = FindFirstObjectByType<PlayerController>().transform;
            yield return new WaitForSeconds(5f);
        }
        targetLock = false;
    }

    private void PhaseOne()
    {

    }

    private void PhaseTwo()
    {

    }

    private void PhaseThree()
    {

    }

    private IEnumerator DashAttack()
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

        yield return new WaitForSeconds(0.5f);

        isDashing = false;
    }

    private IEnumerator ProjectileAttackLoop()
    {
        isThrowingProjectiles = true;
        while (isThrowingProjectiles)
        {
            if (attackPrefabs.Length > 0)
            {
                GameObject projectile = Instantiate(attackPrefabs[0], transform.position, Quaternion.identity);
                Vector3 direction = (target.position - transform.position).normalized;
                FlipSprite(target.position.x);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * projectileSpeed;
                }
            }

            yield return new WaitForSeconds(projectileAttackRate);
        }
    }

    private IEnumerator RetreatPhaseRoutine()
    {
        while (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.5f &&
               enemyHealth.currentHealth > enemyHealth.maxHealth * 0.25f &&
               isReturning)  // Add isReturning check
        {
            // Calculate direction to waypoint
            Vector3 directionToWaypoint = (returnWaypoint.position - transform.position).normalized;
            FlipSprite(directionToWaypoint.x);

            // Move towards waypoint
            transform.position += directionToWaypoint * retreatSpeed * Time.deltaTime;

            // If we reached the waypoint, shoot arc pattern
            if (Vector3.Distance(transform.position, returnWaypoint.position) < 0.5f)
            {
                ShootProjectilesInArc(8, 120f);
                FlipSprite(target.position.x);

                // Wait a bit before next volley
                yield return new WaitForSeconds(2f);

                // Move to a new position around the waypoint
                Vector3 randomOffset = Random.insideUnitSphere * 3f;
                randomOffset.y = 0; // Keep on same Y plane
                Vector3 newPosition = returnWaypoint.position + randomOffset;

                Vector3 position = transform.position;
                position.y = fixedHeight;
                transform.position = position;

                // Move to new position
                while (Vector3.Distance(transform.position, newPosition) > 0.5f &&
                       isReturning &&  // Add isReturning check
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
            // Calculate retreat direction (opposite of player)
            retreatDirection = (transform.position - target.position).normalized;

            // Shoot projectiles while retreating
            GameObject projectile = Instantiate(attackPrefabs[0], transform.position, Quaternion.identity);
            Vector3 directionToPlayer = (target.position - transform.position).normalized;
            FlipSprite(directionToPlayer.x);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = directionToPlayer * projectileSpeed;
            }

            // Move away from player using MoveTowards for more reliable movement
            Vector3 targetPosition = transform.position + (retreatDirection * 10f);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, retreatSpeed * Time.deltaTime);

            // If we're too close to a wall, try to move sideways
            if (Physics.Raycast(transform.position, retreatDirection, 2f))
            {
                // Calculate a perpendicular direction
                Vector3 sideDirection = Vector3.Cross(retreatDirection, Vector3.up).normalized;

                // Randomly choose left or right
                if (Random.value > 0.5f)
                    sideDirection = -sideDirection;

                // Move sideways
                targetPosition = transform.position + (sideDirection * 5f);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, retreatSpeed * Time.deltaTime);
            }

            // Keep a minimum distance from the player
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer < minimumDistance * 2f)
            {
                // Move away faster when too close
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

            // Cancel current state flags
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

        // Resume previous behavior
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
            // Default to projectile attack or normal movement
            StartCoroutine(ProjectileAttackLoop());
        }
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

    private void ShootProjectilesInCircle(int projectileCount)
    {
        float angleStep = 360f / projectileCount;
        float angle = 0f;

        for (int i = 0; i < projectileCount; i++)
        {
            float projectileDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 projectileDirection = new Vector3(projectileDirX, projectileDirY, 0f).normalized;

            GameObject projectile = Instantiate(attackPrefabs[0], transform.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = projectileDirection * 5f;
            }
            else
            {
                Debug.LogError("Projectile has no Rigidbody!");
            }

            angle += angleStep;
        }
    }

    private void ShootProjectilesInArc(int projectileCount, float arcAngle)
    {
        float startAngle = -arcAngle / 2f;
        float angleStep = arcAngle / (projectileCount - 1);

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            float projectileDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectileDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 projectileDirection = new Vector3(projectileDirX, projectileDirY, 0f).normalized;

            GameObject projectile = Instantiate(attackPrefabs[0], transform.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = projectileDirection * 5f;
            }
        }
    }
}
