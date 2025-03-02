using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBossAI : MonoBehaviour
{
    public float speed;
    public float retreatSpeed = 5f;
    public float pushForce = 10f;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public float finalPhaseAttackSpeedMultiplier = 2f; // How much faster attacks become in final phase
    public float homingProjectileSpeed = 8f; // Speed for homing projectiles
    public float homingProjectileLifespan = 4f; // How long homing projectiles last
    public float retreatDistance = 5f;
    public float pushDistance = 2f;
    public Transform target;
    public Animator animator;
    public GameObject[] attackPrefabs;
    public GameObject tornadoPrefab;
    public Transform centerPoint;
    public Transform bossWaypoint;
    public float tornadoSpawnRadius = 3f;
    public float centerPullForce = 5f;
    public float finalPhaseMovementRange = 5f;
    public float centerReachThreshold = 0.5f; // How close player needs to be to center to be considered "centered"

    private EnemyHealth enemyHealth;
    private bool isRetreating;
    private bool isShooting;
    private bool isInTornadoPhase;
    private bool isInFinalPhase;
    private bool isPlayerCentered;
    private bool isHomingEnabled;
    private float tornadoSpawnInterval = 3f;
    private Vector3 originalPlayerPosition;
    private float currentProjectileRate;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (!enemyHealth) Debug.LogError("EnemyHealth component not found!");
        currentProjectileRate = projectileAttackRate;
        StartCoroutine(ProjectileAttackLoop());
    }

    private void Update()
    {
        if (enemyHealth == null) return;

        float healthPercentage = ((float)enemyHealth.currentHealth / enemyHealth.maxHealth) * 100f;

        // Enable homing at 75% health
        if (healthPercentage <= 75f && !isHomingEnabled && !isInFinalPhase)
        {
            isHomingEnabled = true;
        }

        // Phase transitions based on enemy health
        if (healthPercentage <= 50f && !isInTornadoPhase)
        {
            isInTornadoPhase = true;
            StartCoroutine(TornadoPhase());
        }

        if (healthPercentage <= 25f && !isInFinalPhase)
        {
            isInTornadoPhase = false;
            isInFinalPhase = true;
            isHomingEnabled = false; // Disable homing in final phase
            StartFinalPhase();
        }

        if (isInFinalPhase)
        {
            HandleFinalPhase();
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer < pushDistance)
            {
                PushPlayerAway();
            }
            else if (distanceToPlayer < retreatDistance)
            {
                RetreatFromPlayer();
            }
            else
            {
                isRetreating = false;
            }
        }
    }

    private void RetreatFromPlayer()
    {
        isRetreating = true;
        Vector3 retreatDirection = (transform.position - target.position).normalized;
        transform.position += retreatDirection * retreatSpeed * Time.deltaTime;
        FlipSprite(retreatDirection.x);
    }

    private void PushPlayerAway()
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null)
        {
            Vector3 pushDirection = (target.position - transform.position).normalized;
            pushDirection.y = 0; // Ensure no upward movement
            Vector3 pushVelocity = pushDirection * pushForce;
            player.ApplyExternalForce(pushVelocity, 0.5f);
        }
    }

    private IEnumerator TornadoPhase()
    {
        while (isInTornadoPhase)
        {
            SpawnTornadosAroundPlayer();
            yield return new WaitForSeconds(tornadoSpawnInterval);
        }
    }

    private void SpawnTornadosAroundPlayer()
    {
        int tornadoCount = 3;
        for (int i = 0; i < tornadoCount; i++)
        {
            float angle = i * (360f / tornadoCount);
            Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * tornadoSpawnRadius;
            Vector3 spawnPos = target.position + offset;
            spawnPos.y = 0; // Keep tornados at ground level

            Instantiate(tornadoPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void StartFinalPhase()
    {
        if (bossWaypoint != null)
        {
            transform.position = bossWaypoint.position;
        }
        isPlayerCentered = false;
        originalPlayerPosition = centerPoint.position;

        // Increase attack speed in final phase
        currentProjectileRate = projectileAttackRate / finalPhaseAttackSpeedMultiplier;
        StopCoroutine(ProjectileAttackLoop());
        StartCoroutine(ProjectileAttackLoop());
    }

    private void HandleFinalPhase()
    {
        Vector3 playerPos = target.position;
        Vector3 centerPos = centerPoint.position;
        float distanceToCenter = Vector3.Distance(new Vector3(playerPos.x, 0, playerPos.z),
                                                new Vector3(centerPos.x, 0, centerPos.z));

        PlayerController playerController = target.GetComponent<PlayerController>();
        if (playerController == null) return;

        if (!isPlayerCentered)
        {
            // Strong pull towards center until player reaches it
            if (distanceToCenter > centerReachThreshold)
            {
                Vector3 centerDirection = (centerPos - playerPos).normalized;
                Vector3 pullForce = centerDirection * (centerPullForce * 2f); // Stronger initial pull
                playerController.ApplyExternalForce(pullForce, 0.1f);

                // Keep player at same height during pull
                Vector3 currentPos = target.position;
                currentPos.y = centerPos.y;
                target.position = currentPos;
            }
            else
            {
                isPlayerCentered = true;
                // Snap to exact center X position only
                Vector3 centeredPos = target.position;
                centeredPos.x = centerPos.x;
                target.position = centeredPos;
            }
        }
        else
        {
            // Once centered, only restrict X-axis movement within range
            float allowedX = Mathf.Clamp(playerPos.x,
                centerPos.x - finalPhaseMovementRange,
                centerPos.x + finalPhaseMovementRange);

            // Allow free movement on Z axis
            Vector3 restrictedPosition = playerPos;
            restrictedPosition.x = allowedX;
            target.position = restrictedPosition;

            // Light pull force to keep player near center
            Vector3 centerDirection = (centerPos - playerPos).normalized;
            Vector3 pullForce = centerDirection * (centerPullForce * 0.3f); // Lighter maintaining pull
            playerController.ApplyExternalForce(pullForce, 0.1f);
        }
    }

    private void SpawnProjectile()
    {
        if (attackPrefabs.Length > 0)
        {
            GameObject projectileObj = Instantiate(attackPrefabs[Random.Range(0, attackPrefabs.Length)],
                transform.position, Quaternion.identity);

            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.EnableWindProjectile(isHomingEnabled, homingProjectileSpeed, homingProjectileLifespan);
            }
        }
    }

    private IEnumerator ProjectileAttackLoop()
    {
        while (true)
        {
            SpawnProjectile();
            yield return new WaitForSeconds(currentProjectileRate);
        }
    }

    private void FlipSprite(float directionX)
    {
        transform.localScale = new Vector3(Mathf.Sign(directionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
