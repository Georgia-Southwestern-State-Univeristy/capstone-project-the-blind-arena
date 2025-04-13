using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBossStateMachine : MonoBehaviour
{
    public bool isRetreating { get; set; }
    public bool isDead { get; set; }
    public bool isAlive { get; set; }
    public int health { get; set; }
    public int maxHealth { get; set; }
    public int minHealth { get; set; }
    public bool RecoveredFromKnockback { get; set; }

    public FireBossState IdleState { get; private set; }

    [SerializeField] public float speed = 5f;
    public FireBossState currentState;
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance = 5f;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public float projectileSpeed = 8f;
    public float projectileLife = 5f;
    public float retreatSpeed = 10f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float baseAttackRate = 1.5f; // Base attack rate (higher means slower attacks)
    public float finalPhaseAttackRate = 0.8f; // Reduced attack rate for the final phase
    public GameObject[] attackPrefabs;
    public GameObject damageZonePrefab;
    public Animator animator;
    public FireBossState deathState;

    public EnemyHealth enemyHealth;
    private bool isFinalPhase;
    private bool isKnockedBack = false;
    public bool isEnraged = false;
    public bool isThrowingProjectiles = false;
    public bool isDashing = false;
    private float knockbackRecoveryTime = 0.5f;
    private float fixedHeight = 0.6f;

    private bool hasShotProjectiles = false;  // Flag to track if projectiles have been shot
    private bool isMovingToWaypoint = false; // Flag to check if boss is moving to waypoint
    private bool hasReachedWaypoint = false; // Flag to check if waypoint is reached

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

        ChangeState(new IdleState(this));
    }

    private void Update()
    {
        if (!target) return; // Exit early if no target assigned

        if (isKnockedBack) return;

        // Check for entering final phase when health reaches 25% or below
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.25f && !isFinalPhase)
        {
            isFinalPhase = true; // Mark the boss as in the final phase
            ChangeState(new FinalPhaseState(this)); // Switch to final phase state
        }

        // Start moving to waypoint when health reaches 50% (same as before)
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.5f && !isMovingToWaypoint && !hasReachedWaypoint)
        {
            StartCoroutine(MoveToWaypoint());
            return;
        }

        // Handle projectile attacks while health is above 25%
        if (hasReachedWaypoint && !hasShotProjectiles && enemyHealth.currentHealth > enemyHealth.maxHealth * 0.25f)
        {
            StartCoroutine(ShootProjectilesWhileAttacking());
            hasShotProjectiles = true;
        }

        // Transition to RetreatState when health is 25% or lower
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.25f)
        {
            if (isThrowingProjectiles)
            {
                StopCoroutine(ShootProjectilesWhileAttacking());
                isThrowingProjectiles = false;
            }

            // Transition to RetreatState if health is 25% or lower
            if (!(currentState is RetreatState))
            {
                ChangeState(new RetreatState(this));
            }
        }

        currentState.UpdateState();
    }

    // Coroutine to move the boss to the waypoint
    private IEnumerator MoveToWaypoint()
    {
        isMovingToWaypoint = true;

        while (Vector3.Distance(transform.position, returnWaypoint.position) > minimumDistance)
        {
            // Move the boss toward the waypoint
            Vector3 direction = (returnWaypoint.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }

        hasReachedWaypoint = true;  // Set flag to true once waypoint is reached
        isMovingToWaypoint = false; // No longer moving
    }

    // Coroutine to shoot projectiles in an arc while health is above 25%
    private IEnumerator ShootProjectilesWhileAttacking()
    {
        isThrowingProjectiles = true;

        while (enemyHealth.currentHealth > enemyHealth.maxHealth * 0.25f)
        {
            ShootProjectilesInArc(8, 120f);  // Shoot projectiles in arc
            yield return new WaitForSeconds(currentState is FinalPhaseState ? baseAttackRate : projectileAttackRate);  // Adjusted attack rate
        }
    }


    // Method to handle the projectile attack logic
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
                rb.linearVelocity = projectileDirection * projectileSpeed;  // Adjust speed as needed
            }
        }
    }

    public void ChangeState(FireBossState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = newState;
        currentState.EnterState();

        if (currentState is IdleState)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; // Stop all movement (if needed)
            }
        }
    }

    public void OnKnockback()
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            ChangeState(new KnockbackState(this));
            StartCoroutine(KnockbackRecovery());
        }
    }

    public void ApplyKnockbackForce()
    {
        // Add the logic for applying knockback force here
        Debug.Log("Applying knockback force");
    }

    private IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(knockbackRecoveryTime);
        isKnockedBack = false;

        if (isFinalPhase)
        {
            ChangeState(new FinalPhaseState(this));
        }
        else if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.5f)
        {
            ChangeState(new RetreatState(this));
        }
        else
        {
            ChangeState(new ProjectileAttackState(this));
        }
    }

    public void SetRecoveredFromKnockback(bool value)
    {
        RecoveredFromKnockback = value;
    }

    public void FlipSprite(float directionX)
    {
        transform.localScale = new Vector3(Mathf.Sign(directionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void PerformFinalPhaseAttacks()
    {
        // Implement unique behavior for the final phase
        Debug.Log("Boss is attacking aggressively in the final phase!");
    }

    internal void StartProjectileAttack()
    {
        // Implement logic for initiating projectile attack if needed
    }
}
