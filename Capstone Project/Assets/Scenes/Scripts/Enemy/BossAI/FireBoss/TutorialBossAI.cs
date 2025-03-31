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
    private bool isThrowingProjectiles;
    private bool isKnockedBack = false;
    private bool targetLock=false;
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

        // If being knocked back, don't run normal movement logic
        if (isKnockedBack)
        {
            return;
        }

        CheckForTarget();

        // Normal phase
        if (true)
        {
            // Start shooting if not already
            if (!isThrowingProjectiles)
            {
                StartCoroutine(ShootProjectilesInArc());
            }

            // Handle movement
            if (Vector3.Distance(transform.position, target.position) > minimumDistance)
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

    private IEnumerator ProjectileAttackLoop()
    {
        isThrowingProjectiles = true;
        while (isThrowingProjectiles)
        {
            if (attackPrefabs.Length > 0)
            {
                GameObject projectile = Instantiate(attackPrefabs[0], transform.position, Quaternion.identity);
                ProjectileAttack attack = projectile.GetComponent<ProjectileAttack>();
                attack.target = target;
                FlipSprite(target.position.x);
            }

            yield return new WaitForSeconds(projectileAttackRate);
        }
    }

    private IEnumerator ShootProjectilesInArc()
    {
        int arcAngle = 75, projectileCount = 5;
        float startAngle = -arcAngle / 2f;
        float angleStep = arcAngle / (projectileCount - 1);

        isThrowingProjectiles = true;
        while (isThrowingProjectiles)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = startAngle + (angleStep * i);
                Vector3 spreadDirection = Quaternion.Euler(0, angle, 0) * directionToTarget;

                GameObject projectile = Instantiate(attackPrefabs[0], transform.position, Quaternion.identity);
                ProjectileAttack attack = projectile.GetComponent<ProjectileAttack>();
                attack.Init(target.transform, spreadDirection, 10);
            }
            yield return new WaitForSeconds(projectileAttackRate);
        }
    }

    private IEnumerator CheckForTarget()
    {
        if (!targetLock)
        {
            targetLock=true;
            target = FindFirstObjectByType<PlayerController>().transform;
            yield return new WaitForSeconds(5f);
        }
        targetLock=false;
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
