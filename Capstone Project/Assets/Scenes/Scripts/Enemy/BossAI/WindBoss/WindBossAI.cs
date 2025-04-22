using System;
using System.Collections;
using UnityEngine;

public class WindBossAI : MonoBehaviour
{
    public Transform target;
    public Transform returnWaypoint;
    public Animator animator;
    public float speed = 6.5f;
    public float minimumDistance = 7f;
    public float retreatDistance = 3f;
    public float retreatSpeed = 5f;
    public float pushDistance = 2f;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public float tornadoSpawnRadius = 3f;
    public float finalPhaseMovementRange = 5f;
    public float centerReachThreshold = 0.5f; // How close player needs to be to center to be considered "centered"
    public GameObject[] attackPrefabs;

    private EnemyHealth enemyHealth;
    private BossCameraSwitcher boardSwitcher;
    private bool startPhaseOne = false;
    private bool startPhaseTwo = false;
    private bool startPhaseThree = false;
    private bool startPhaseFour = false;
    private bool startSetup = false;
    private bool isRetreating = false;
    private bool interruptMovement;
    private bool attackLock, proLock, stopRetreat;
    private bool targetLock = false;
    private float moveCounter = 0f;
    private const float HEIGHT = 0.6f;
    private System.Random rnd = new System.Random();
    private int random;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        Rigidbody rb = GetComponent<Rigidbody>();
        boardSwitcher = GetComponent<BossCameraSwitcher>();

        if (!enemyHealth) Debug.LogError("EnemyHealth component not found!");
    }

    private void Update()
    {
        if (!targetLock)
        {
            targetLock = true;
            StartCoroutine(CheckForTarget());
        }
        // Enable Final Phase at 25% health

        // Enable Tornadoes at 50% health

        // Enable homing at 75% health
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.75f)
        {
            if (!startPhaseTwo)
            {
                startPhaseTwo = true;
                StopAllCoroutines();
                interruptMovement = false;
                proLock = false;
                attackLock = false;
                targetLock = false;
                StartCoroutine(PhaseTwo());
            }
            if (Vector3.Distance(transform.position, target.position) > minimumDistance && !interruptMovement)
            {
                MoveTowardsPlayer();
            }
            else
            {
                animator.SetFloat("speed", 0);
            }
            return;
        }
        else 
        {
            if (!startPhaseOne)
            {
                startPhaseOne = true;
                interruptMovement = false;
                attackLock = false;
                proLock = false;
                StartCoroutine(PhaseOne());
            }
            if (Vector3.Distance(transform.position, target.position) > minimumDistance && !interruptMovement && !isRetreating)
            {
                MoveTowardsPlayer();
            }
            else if (Vector3.Distance(transform.position, target.position) < retreatDistance && !interruptMovement && !isRetreating)
            {
                StartCoroutine(RetreatFromPlayer());
            }
            else
            {
                animator.SetFloat("speed", 0);
            }
            return;
        }
    }

    private IEnumerator CheckForTarget()
    {
        System.Random rand = new System.Random();
        int newTarg = rand.Next(0, 3);
        switch (newTarg)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
        target = FindFirstObjectByType<PlayerController>().transform;
        yield return new WaitForSeconds(10f);
        targetLock=false;
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction *= ((Math.Abs(direction.z) * .6f) + 1) * speed;
        animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - direction.magnitude));
        FlipSprite(direction.x);

        Vector3 position = transform.position;
        position.y = HEIGHT;
        transform.position = position;

        transform.position += direction * Time.deltaTime;
    }

    private IEnumerator RetreatFromPlayer()
    {
        if (!isRetreating)
        {
            Debug.Log("Start Retreat");
            isRetreating = true;
            interruptMovement = true;
            yield return new WaitForSeconds(0.5f);
            Vector3 dashDirection = -(target.position - transform.position).normalized;
            int newTarg = rnd.Next(0, 2);
            switch (newTarg)
            {
                case 0:
                    break;
                case 1:
                    dashDirection = (returnWaypoint.position - transform.position).normalized;
                    break;
            }
            Vector3 dir = dashDirection;
            dir.y = 0;
            dashDirection = dir;
            dashDirection *= ((Math.Abs(dashDirection.z) * .6f) + 1);
            float elapsedTime = 0f;
            FlipSprite(dashDirection.x);
            if (startPhaseTwo)
            {
                StartCoroutine(RetreatAttack());
            }
            while (elapsedTime < .6f && !stopRetreat)
            {
                transform.position += dashDirection * 20 * Time.deltaTime;
                FlipSprite(dashDirection.x);
                animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - dashDirection.magnitude));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            StopCoroutine(RetreatAttack());
            yield return new WaitForSeconds(1);
            interruptMovement = false;
            isRetreating = false;
        }
    }

    private IEnumerator PhaseOne()
    {
        Debug.Log("Phase One");
        while (startPhaseOne)
        {
            if (!attackLock)
            {
                random = rnd.Next(0, 3);
                Debug.Log(random);
            }
            attackLock = true;
            yield return new WaitForSeconds(1f);
            if (random == 0)
            {
                if (!proLock)
                {
                    StartCoroutine(ShootProjectile(0, 6, 0.5f));
                }
                yield return new WaitForSeconds(8f);
            }
            else
            {
                if (!proLock)
                {
                    StartCoroutine(ProjectileArc(40, 3, 0, 1));
                }
            }
        }
        yield return new WaitForSeconds(0.01f);
    }

    private IEnumerator PhaseTwo()
    {

        yield return new WaitForSeconds(0.01f);
    }

    private IEnumerator RetreatAttack()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject projectile = Instantiate(attackPrefabs[4], transform.position, Quaternion.identity);
            yield return new WaitForSeconds(.6f/5);
        }
    }

    private IEnumerator ShootProjectile(int type, int amount, float wait)
    {
        proLock = true;
        for (int i = 0; i < amount; i++)
        {
            if (attackPrefabs.Length > type)
            {
                animator.SetTrigger("Gun");
                interruptMovement = true;
                yield return new WaitForSeconds(0.2f);
                GameObject projectile = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                ProjectileAttack attack = projectile.GetComponent<ProjectileAttack>();
                attack.target = target;
                FlipSprite((target.position - transform.position).normalized.x);
                yield return new WaitForSeconds(0.1f);
                interruptMovement = false;
            }
            yield return new WaitForSeconds(projectileAttackRate / wait);
        }
        proLock = false;
    }

    private IEnumerator ProjectileArc(int arc, int count, int type, int amount)
    {
        proLock = true;
        int arcAngle = arc, projectileCount = count;
        float startAngle = -arcAngle / 2f;
        float angleStep = arcAngle / (projectileCount - 1);

        for (int i = 0; i < amount; i++)
        {
            animator.SetTrigger("Burst");
            interruptMovement = true;
            yield return new WaitForSeconds(0.2f);
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            FlipSprite(directionToTarget.x);
            for (int j = 0; j < projectileCount; j++)
            {
                float angle = startAngle + (angleStep * j);
                Vector3 spreadDirection = Quaternion.Euler(0, angle, 0) * directionToTarget;

                GameObject projectile = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                ProjectileAttack attack = projectile.GetComponent<ProjectileAttack>();
                attack.Init(target.transform, spreadDirection);
            }
            yield return new WaitForSeconds(0.1f);
            interruptMovement = false;
            yield return new WaitForSeconds(projectileAttackRate);
        }
        proLock = false;
    }

    private IEnumerator HomingArc(int arc, int count, int type, int amount)
    {
        proLock = true;
        int arcAngle = arc, projectileCount = count;
        float startAngle = -arcAngle / 2f;
        float angleStep = arcAngle / (projectileCount - 1);

        for (int i = 0; i < amount; i++)
        {
            animator.SetTrigger("Sweep");
            interruptMovement = true;
            yield return new WaitForSeconds(0.2f);
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            FlipSprite(directionToTarget.x);
            for (int j = 0; j < projectileCount; j++)
            {
                float angle = startAngle + (angleStep * j);
                Vector3 spreadDirection = Quaternion.Euler(0, angle, 0) * directionToTarget;

                GameObject projectile = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                ProjectileAttack attack = projectile.GetComponent<ProjectileAttack>();
                attack.Init(target.transform, spreadDirection);
            }
            yield return new WaitForSeconds(0.1f);
            interruptMovement = false;
            yield return new WaitForSeconds(projectileAttackRate);
        }
        proLock = false;
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
        if (collision.CompareTag("Wall"))
        {
            Debug.Log("Enemy In Wall");
            stopRetreat = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        stopRetreat = false;
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("Enemy In Wall");
            stopRetreat = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        stopRetreat = false;
    }

    private void HandlePlayerCollision(GameObject player)
    {
        Debug.Log("Boss hit the player!");
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.Damage(20);
        }
    }

    /*
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
    */
}
