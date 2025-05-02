using System;
using System.Collections;
using UnityEngine;

public class WindBossAI : MonoBehaviour
{
    public Transform target;
    public Transform returnWaypoint;
    public Transform topWaypoint;
    public Transform bottomWaypoint;
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
    private Transform targetWaypoint;
    private bool startPhaseOne = false;
    private bool startPhaseTwo = false;
    private bool startPhaseThree = false;
    private bool startPhaseFour = false;
    private bool startSetup = false;
    private bool isRetreating = false;
    private bool interruptMovement;
    private bool attackLock, proLock, stopRetreat, isSetup;
    private bool targetLock = false;
    private float moveCounter = 0f;
    private const float HEIGHT = 0.6f;
    private System.Random rnd = new System.Random();
    private int random;

    [SerializeField] private AudioSource walkingAudioSource;

    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioSource sfxAudioSource;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        Rigidbody rb = GetComponent<Rigidbody>();
        boardSwitcher = GetComponent<BossCameraSwitcher>();
        targetWaypoint = topWaypoint;

        if (!enemyHealth) Debug.LogError("EnemyHealth component not found!");
    }

    private void Update()
    {
        if (!targetLock)
        {
            targetLock = true;
            StartCoroutine(CheckForTarget());
        }

        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Enable Final Phase at 25% health
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.25f)
        {
            if (!startPhaseFour)
            {
                startPhaseFour = true;
                StopAllCoroutines();
                interruptMovement = false;
                attackLock = false;
                proLock = false;
                isRetreating = false;
                targetLock = false;
                startSetup = false;
                StartCoroutine(PhaseFour());
            }
            if (Vector3.Distance(transform.position, target.position) < retreatDistance && !interruptMovement && !isRetreating && isSetup)
            {
                StartCoroutine(SwapArenaSides());
            }
            else if (Vector3.Distance(transform.position, targetWaypoint.position) > 1 && !interruptMovement && !isRetreating && isSetup)
            {
                MoveTowardsWaypoint();
            }
            else if (!startSetup)
            {
                startSetup = true;
                StartCoroutine(SetupArena());
            }
            else
            {
                animator.SetFloat("speed", 0);

                if (walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.Stop();
                }
            }
            return;
        }
        else
        // Enable Tornadoes + Retreat attack at 50% health
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.50f)
        {
            if (!startPhaseThree)
            {
                startPhaseThree = true;
                StopAllCoroutines();
                interruptMovement = false;
                attackLock = false;
                proLock = false;
                isRetreating = false;
                targetLock = false;
                StartCoroutine(PhaseThree());
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

                if (walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.Stop();
                }
            }
            return;
        }
        // Enable homing + Burst Attack at 75% health
        else if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.75f)
        {
            if (!startPhaseTwo)
            {
                startPhaseTwo = true;
                StopAllCoroutines();
                interruptMovement = false;
                attackLock = false;
                proLock = false;
                isRetreating = false;
                targetLock = false;
                StartCoroutine(PhaseTwo());
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

                if (walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.Stop();
                }
            }
            return;
        }
        else 
        {
            // Projectile Attacks + Arcs
            if (!startPhaseOne)
            {
                startPhaseOne = true;
                interruptMovement = false;
                attackLock = false;
                proLock = false;
                isRetreating = false;
                targetLock = false;
                StartCoroutine(PhaseOne());
            }
            if (target != null && target.gameObject != null && Vector3.Distance(transform.position, target.position) > minimumDistance && !interruptMovement)
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

                if (walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.Stop();
                }
            }
            return;
        }
    }

    private IEnumerator CheckForTarget()
    {
        PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();

        if (playerControllers.Length == 0)
        {
            Debug.LogWarning("No players found. Skipping target selection.");
            target = null;
            yield return new WaitForSeconds(10f);
            targetLock = false;
            yield break; // Exit the coroutine early
        }

        int newTarg = rnd.Next(0, playerControllers.Length);
        Debug.Log("Target Check: " + newTarg);
        target = playerControllers[newTarg].transform;
        yield return new WaitForSeconds(10f);
        targetLock = false;
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

        if (!walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Play();
        }
    }

    private void MoveTowardsWaypoint()
    {
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        direction *= ((Math.Abs(direction.z) * .6f) + 1) * speed;
        animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - direction.magnitude));
        FlipSprite(direction.x);

        Vector3 position = transform.position;
        position.y = HEIGHT;
        transform.position = position;

        transform.position += direction * Time.deltaTime;

        if (!walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Play();
        }
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
            if (startPhaseTwo && !startPhaseThree)
            {
                StartCoroutine(ProjectileArc(2, 360, 9, 1, projectileAttackRate, 1));
                yield return new WaitForSeconds(projectileAttackRate+1.5f);
            }
            if (startPhaseThree)
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
            if (startPhaseThree)
            {
                StopCoroutine(RetreatAttack());
            }
            yield return new WaitForSeconds(2);
            interruptMovement = false;
            isRetreating = false;
        }
    }

    private IEnumerator SwapArenaSides()
    {
        if (!isRetreating)
        {
            if (targetWaypoint == topWaypoint)
            {
                targetWaypoint = bottomWaypoint;
            }
            else
            {
                targetWaypoint=topWaypoint;
            }
            Debug.Log("Start Retreat");
            isRetreating = true;
            interruptMovement = true;
            yield return new WaitForSeconds(0.5f);
            Vector3 dashDirection = (targetWaypoint.position - transform.position).normalized;
            Vector3 dir = dashDirection;
            dir.y = 0;
            dashDirection = dir;
            dashDirection *= ((Math.Abs(dashDirection.z) * 1f) + 1);
            float elapsedTime = 0f;
            FlipSprite(dashDirection.x);
            StartCoroutine(ProjectileArc(2, 360, 9, 1, projectileAttackRate, 1));
            while (elapsedTime < 1f)
            {
                transform.position += dashDirection * 20 * Time.deltaTime;
                FlipSprite(dashDirection.x);
                animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - dashDirection.magnitude));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(2);
            interruptMovement = false;
            isRetreating = false;
        }
    }

    private IEnumerator PhaseOne()
    {
        Debug.Log("Phase One");
        while (startPhaseOne)
        {
            while (isRetreating)
                yield return new WaitForSeconds(1f);
            if (!attackLock)
            {
                random = rnd.Next(0, 3);
                Debug.Log(random);
            }
            attackLock = true;

            if (!target || target.gameObject == null)
            {
                Debug.Log("Target destroyed during PhaseOne, aborting behavior.");
                yield break; // Safely exit the coroutine
            }

            yield return new WaitForSeconds(1f);
            if (random == 0) // Single Bullets
            {
                if (!proLock)
                {
                    StartCoroutine(ShootProjectile(2, 3, projectileAttackRate, 0));
                }
            }
            else
            {
                // Bullet Arc
                if (!proLock)
                {
                    StartCoroutine(ProjectileArc(5, 40, 3, 1, projectileAttackRate, 1));
                }
            }
        }
        yield return new WaitForSeconds(0.01f);
    }

    private IEnumerator PhaseTwo()
    {
        while (startPhaseTwo)
        {
            while (isRetreating)
                yield return new WaitForSeconds(1f);
            if (!attackLock)
            {
                random = rnd.Next(0, 3);
                Debug.Log(random);
            }
            attackLock = true;
            yield return new WaitForSeconds(1f);
            switch (random)
            {
                case 0: // Bullet Stream
                    if (!proLock)
                    {
                        StartCoroutine(ShootProjectile(0, 1, .5f, 0));
                    }
                    break;
                case 1: // Homing Attack
                    if (!proLock)
                    {
                        StartCoroutine(ProjectileArc(4, 60, 3, 1, projectileAttackRate, 2));
                    }
                    break;
                case 2: // Bullet Ring
                    if (!proLock)
                    {
                        StartCoroutine(ProjectileArc(3, 360, 9, 1, projectileAttackRate, 1));
                    }
                    break;
            }
        }
        yield return new WaitForSeconds(0.01f);
    }

    private IEnumerator PhaseThree()
    {
        int counter=0;
        while (startPhaseThree)
        {
            while (isRetreating)
                yield return new WaitForSeconds(1f);
            if (!attackLock)
            {
                if (counter > 7)
                    counter = 0;
                if (counter == 0) // Summon Tornadoes
                {
                    StartCoroutine(ProjectileArc(6, 180, 3, 1, 0, 2));
                    yield return new WaitForSeconds(2f);
                }
                random = rnd.Next(0, 4);
                Debug.Log(random);
            }
            attackLock = true;
            yield return new WaitForSeconds(1f);
            switch (random)
            {
                case 0: // Bullet Stream
                    if (!proLock)
                    {
                        StartCoroutine(ProjectileArc(2, 90, 3, 2, projectileAttackRate/3, 0));
                        counter++;
                    }
                    break;
                case 1: // Homing Attack
                    if (!proLock)
                    {
                        StartCoroutine(ProjectileArc(4, 120, 3, 1, projectileAttackRate, 2));
                        counter++;
                    }
                    break;
                case 2: // Bullet Ring
                    if (!proLock)
                    {
                        StartCoroutine(ProjectileArc(3, 360, 9, 1, projectileAttackRate/3, 1));
                        counter++;
                    }
                    break;
                case 3: // Slicer Arc
                    if (!proLock)
                    {
                        StartCoroutine(ProjectileArc(5, 120, 5, 1, projectileAttackRate, 2));
                        counter++;
                    }
                    break;
            }
        }
        yield return new WaitForSeconds(0.01f);
    }

    private IEnumerator PhaseFour()
    {
        while (startPhaseFour)
        {
            while (isRetreating)
                yield return new WaitForSeconds(1f);
            if (isSetup)
            {
                    if (!proLock)
                    {
                        StartCoroutine(WindExtras());
                    }
                    if (Vector3.Distance(transform.position, targetWaypoint.position) < 1 && !attackLock)
                    {
                        StartCoroutine(FastShot(1, 1, 0));
                    }
                    yield return new WaitForSeconds(1.2f);
            }
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(0.01f);
    }

    private IEnumerator RetreatAttack()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject projectile = Instantiate(attackPrefabs[5], transform.position, Quaternion.identity);
            yield return new WaitForSeconds(.6f/5);
            PlayAttackSound(6);
        }
    }

    private IEnumerator ShootProjectile(int type, int amount, float wait, int anim)
    {
        proLock = true;
        for (int i = 0; i < amount; i++)
        {
            if (attackPrefabs.Length > type)
            {
                interruptMovement = true;
                switch (anim)
                {
                    case 0:
                        animator.SetTrigger("Gun");
                        yield return new WaitForSeconds(.66f);
                        break;
                    case 1:
                        animator.SetTrigger("Burst");
                        yield return new WaitForSeconds(.58f);
                        break;
                    case 2:
                        animator.SetTrigger("Sweep");
                        yield return new WaitForSeconds(.42f);
                        break;
                }
                GameObject projectile = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                ProjectileAttack attack = projectile.GetComponent<ProjectileAttack>();
                attack.target = target;
                if (target != null)
                {
                    FlipSprite((target.position - transform.position).normalized.x);
                }
                yield return new WaitForSeconds(.5f);
                interruptMovement = false;
            }
            yield return new WaitForSeconds(wait);
        }
        attackLock = false;
        proLock = false;
    }

    private IEnumerator ProjectileArc(int type, int arc, int count, int amount, float wait, int anim)
    {
        proLock = true;
        int arcAngle = arc, projectileCount = count;
        float startAngle = -arcAngle / 2f;
        float angleStep = arcAngle / (projectileCount - 1);

        for (int i = 0; i < amount; i++)
        {
            interruptMovement = true;
            switch (anim)
            {
                case 0:
                    animator.SetTrigger("Gun");
                    yield return new WaitForSeconds(.66f);
                    break;
                case 1:
                    animator.SetTrigger("Burst");
                    yield return new WaitForSeconds(.58f);
                    break;
                case 2:
                    animator.SetTrigger("Sweep");
                    yield return new WaitForSeconds(.42f);
                    break;
            }
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
            yield return new WaitForSeconds(.5f);
            interruptMovement = false;
            yield return new WaitForSeconds(wait);
        }
        attackLock = false;
        proLock = false;
    }

    private IEnumerator WindExtras()
    {
        proLock = true;
        int counter = 0;
        while (true) 
        {
            int proPattern = rnd.Next(0, 2); // Projectile Pattern
            int proAmount = rnd.Next(0, 5); // Projectile Amount
            switch (proPattern)
            {
                case 0:
                    for (int i = 0; i< proAmount + 1 + counter; i++)
                    {
                        Instantiate(attackPrefabs[2], returnWaypoint.position + new Vector3(-17, -1f, 0 + i*1.5f), new Quaternion(0, 0, 0, 0));
                        Instantiate(attackPrefabs[2], returnWaypoint.position + new Vector3(17, -1f, 0 + i * 1.5f), new Quaternion(0, 0, 0, 0));
                        yield return new WaitForSeconds(0.5f);
                    }
                    break;
                case 1:
                    for (int i = 0; i < proAmount + 1 + counter; i++)
                    {
                        Instantiate(attackPrefabs[2], returnWaypoint.position + new Vector3(-17, -1f, 0 + i * 1.5f), new Quaternion(0, 0, 0, 0));
                        Instantiate(attackPrefabs[2], returnWaypoint.position + new Vector3(17, -1f, 0 - i * 1.5f), new Quaternion(0, 0, 0, 0));
                        yield return new WaitForSeconds(0.5f);
                    }
                    break;
            }
            counter++;
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator FastShot(int type, int amount, float wait)
    {
        GameObject projectile, projectile1, projectile2;
        ProjectileAttack attack, attack1, attack2;
        attackLock = true;
        for (int i = 0; i < amount; i++)
        {
            if (attackPrefabs.Length > type)
            {
                interruptMovement = true;
                animator.SetTrigger("Fast");
                yield return new WaitForSeconds(.42f);
                if (isRetreating)
                {
                    i = amount;
                    break;
                }
                projectile = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                attack = projectile.GetComponent<ProjectileAttack>();
                attack.target = target;
                FlipSprite((target.position - transform.position).normalized.x);
                yield return new WaitForSeconds(.25f);
                if (isRetreating)
                {
                    i = amount;
                    break;
                }
                projectile1 = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                attack1 = projectile1.GetComponent<ProjectileAttack>();
                attack1.target = target;
                FlipSprite((target.position - transform.position).normalized.x);
                yield return new WaitForSeconds(.25f);
                if (isRetreating)
                {
                    i = amount;
                    break;
                }
                projectile2 = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                attack2 = projectile2.GetComponent<ProjectileAttack>();
                attack2.target = target;
                FlipSprite((target.position - transform.position).normalized.x);
                yield return new WaitForSeconds(.5f);
                interruptMovement = false;
            }
            yield return new WaitForSeconds(wait);
        }
        attackLock = false;
    }

    public IEnumerator SetupArena()
    {
        //Camera focus on Boss
        boardSwitcher.focusOnBoss = true;
        PlayerController[] playerController = FindObjectsOfType<PlayerController>();
        SinglePlayerAttack[] singlePlayerAttacks = FindObjectsOfType<SinglePlayerAttack>();
        GameObject player;

        //Lock player movement and abilities
        foreach (PlayerController playerCon in playerController)
            playerCon.LockMovement(10f);
        foreach (SinglePlayerAttack singlePlayerAttack in singlePlayerAttacks)
            singlePlayerAttack.attackChecker = false;
        ProjectileCleaner.DestroyAllProjectiles();

        //Move to waypoint
        while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.5f)
        {
            MoveTowardsWaypoint();
            yield return new WaitForSeconds(0.001f);
        }

        //Summon Wind Walls
        interruptMovement = true;
        animator.SetTrigger("Raise");
        yield return new WaitForSeconds(1);

        for (int i = 0; i < 5; i++)
        {
            foreach (PlayerController playerCon in playerController)
            {
                player = playerCon.gameObject;
                player.transform.position = transform.position + new Vector3(0, 0, -20);
                yield return new WaitForSeconds(0.1f);
            }
            Debug.Log("Attempt Player Move");
            yield return new WaitForEndOfFrame();
        }

        Instantiate(attackPrefabs[7], returnWaypoint.position + new Vector3(-15, -1f, 0), new Quaternion(0, 0, 0, 0));
        Instantiate(attackPrefabs[8], returnWaypoint.position + new Vector3(15, -1f, 0), new Quaternion(0, 180, 0, 0));

        yield return new WaitForSeconds(1f);
        boardSwitcher.focusOnBoss = false;
        foreach (PlayerController playerCon in playerController)
            playerCon.UnlockMovement();
        yield return new WaitForSeconds(2f);
        interruptMovement = false;
        isSetup = true;
        foreach (SinglePlayerAttack singlePlayerAttack in singlePlayerAttacks)
            singlePlayerAttack.attackChecker = true;
        PlayAttackSound(0);
        // StartCoroutine(PullPlayerAndTrap());
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
        if (collision.CompareTag("Wall"))
        {
            stopRetreat = false;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Wall"))
            stopRetreat = true;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
        if (collision.collider.CompareTag("Wall"))
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

        if (collision.collider.CompareTag("Wall"))
            stopRetreat = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
            stopRetreat = true;
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

    public void PlayAttackSound(int soundIndex)
    {
        if (attackSounds != null && soundIndex >= 0 && soundIndex < attackSounds.Length)
        {
            sfxAudioSource.PlayOneShot(attackSounds[soundIndex], sfxAudioSource.volume * 0.3f); // Reduce volume by 30%
        }
        else
        {
            Debug.LogWarning($"Attack sound at index {soundIndex} is not assigned!");
        }
    }

}
