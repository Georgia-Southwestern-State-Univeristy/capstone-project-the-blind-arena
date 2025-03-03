using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossAI : MonoBehaviour
{
    public float speed;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance = 5f;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public float projectileSpeed = 8f;
    public float projectileLife = 5f;
    public Animator animator;
    public GameObject[] attackPrefabs;

    private EnemyHealth enemyHealth;
    private bool isDashing;
    private bool isReturning;
    private bool isThrowingProjectiles;
    private bool isKnockedBack = false;
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
            Debug.LogWarning("No target assigned to TutorialBoss!");
            return;
        }

        // If being knocked back, don't run normal movement logic
        if (isKnockedBack)
        {
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

    public void OnKnockback()
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            StopAllCoroutines();
            StartCoroutine(KnockbackRecovery());
        }
    }

    private IEnumerator KnockbackRecovery()
    {
        yield return new WaitForSeconds(knockbackRecoveryTime);
        isKnockedBack = false;
        StartCoroutine(ProjectileAttackLoop());
    }

    private void FlipSprite(float directionX)
    {
        transform.localScale = new Vector3(Mathf.Sign(directionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
