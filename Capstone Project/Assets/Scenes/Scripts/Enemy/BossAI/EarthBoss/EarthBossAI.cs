using System;
using System.Collections;
using UnityEngine;

public class EarthBossAI : MonoBehaviour
{
    public Transform target;
    public Transform returnWaypoint;
    public Transform targetWaypoint;
    public Transform playerWaypoint;
    public Animator animator;
    public float speed;
    public float minimumDistance = 2f;
    public float attackDelay = 2f;
    public float leapCooldown = 5f;
    public float spikeCooldown = 3f;
    public float pullCooldown = 8f;
    public GameObject[] attackPrefabs;

    private EnemyHealth enemyHealth;
    private BossCameraSwitcher boardSwitcher;
    private bool startPhaseOne = false;
    private bool startPhaseTwo = false;
    private bool startPhaseThree = false;
    private bool startPhaseFour = false;
    private bool startSetup = false;
    private bool isSetup = false;
    private bool isLeaping = false;
    private bool isStomping = false;
    private bool interruptMovement;
    private bool attackLock, proLock, stopLeap;
    private bool targetLock = false;
    private const float HEIGHT = 0.6f;
    private System.Random rnd = new System.Random();
    private int random;

    [SerializeField] private AudioSource walkingAudioSource;

    [SerializeField] private AudioClip[] attackSounds; // 0 = Melee, 1 = Leap, 2 = Stomp, 3 = Pull
    [SerializeField] private AudioSource sfxAudioSource;

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

        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.25f)
        {
            // Phase 4: Move to waypoint and start pull/trap sequence
            if (!startPhaseFour)
            {
                startPhaseFour = true;
                StopAllCoroutines();
                interruptMovement = false;
                attackLock = false;
                proLock = false;
                targetLock = false;
                isLeaping = false;
                startSetup = false;
                StartCoroutine(PhaseFour());
            }
            if (Vector3.Distance(transform.position, target.position) > minimumDistance && !interruptMovement && isSetup)
            {
                MoveTowardsPlayer();
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
        else if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.50f)
        {
            // Phase 3: Disable leaping, enable spike attack
            if (!startPhaseThree)
            {
                startPhaseThree = true;
                StopAllCoroutines();
                interruptMovement = false;
                proLock = false;
                attackLock = false;
                targetLock = false;
                isLeaping = false;
                StartCoroutine(PhaseThree());
            }
            if (Vector3.Distance(transform.position, target.position) > minimumDistance && !interruptMovement)
            {
                MoveTowardsPlayer();
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
        else if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.75f)
        {
            // Phase 2: Enable Leaping, more aggression
            if (!startPhaseTwo)
            {
                startPhaseTwo = true;
                StopAllCoroutines();
                interruptMovement = false;
                proLock = false;
                attackLock = false;
                targetLock = false;
                speed += 1;
                StartCoroutine(PhaseTwo());
            }
            if (Vector3.Distance(transform.position, target.position) > minimumDistance && !interruptMovement)
            {
                MoveTowardsPlayer();
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
            // Phase 1: Melee Combat
            if (!startPhaseOne)
            {
                startPhaseOne = true;
                interruptMovement = false;
                attackLock = false;
                proLock = false;
                StartCoroutine(PhaseOne());
            }
            if (Vector3.Distance(transform.position, target.position) > minimumDistance && !interruptMovement)
            {
                MoveTowardsPlayer();
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
        int newTarg = rnd.Next(0, playerControllers.Length);
        Debug.Log("Target Check: "+ newTarg);
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

    private void MoveTowardsWapoint()
    {
        Vector3 direction = (returnWaypoint.position - transform.position).normalized;
        direction *= ((Math.Abs(direction.z) * .6f) + 1) * speed;
        animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - direction.magnitude));
        FlipSprite(direction.x);

        Vector3 position = transform.position;
        position.y = HEIGHT;
        transform.position = position;

        transform.position += direction * Time.deltaTime;
    }

    private IEnumerator PhaseOne()
    {
        while (startPhaseOne)
        {
            if (!attackLock)
            {
                attackLock = true;
                if (Vector3.Distance(transform.position, target.position) <= minimumDistance + 0.1f)
                {
                    StartCoroutine(PerformMeleeAttack());
                    yield return new WaitForSeconds(1.5f);
                    interruptMovement = false;
                }
                attackLock = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator PhaseTwo()
    {
        while (startPhaseTwo)
        {
            if (!attackLock)
            {
                attackLock = true;
                yield return new WaitForSeconds(1f);
                isLeaping = true;
                StartCoroutine(PerformLeapAttack());
                yield return new WaitForSeconds(leapCooldown);
                interruptMovement = false;
                for (int i = 0; i < 30; i++)
                {
                    if (Vector3.Distance(transform.position, target.position) <= minimumDistance + 0.1f)
                    {
                        StartCoroutine(PerformMeleeAttack());
                        yield return new WaitForSeconds(1.1f);
                        interruptMovement = false;
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                attackLock = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator PhaseThree()
    {
        while (startPhaseThree)
        {
            if (!attackLock)
            {
                random = rnd.Next(0, 3);
                Debug.Log(random);
            }
            attackLock = true;
            yield return new WaitForSeconds(1f);
            isStomping = true;
            StartCoroutine(PerformStompAttack());
            yield return new WaitForSeconds(3f);
            interruptMovement = false;
            switch (random)
            {
                case 0:
                    isStomping = true;
                    StartCoroutine(PerformStompAttack());
                    yield return new WaitForSeconds(3f);
                    interruptMovement = false; 
                    break;
                case 1:
                    for (int i = 0; i < 20; i++)
                    {
                        if (Vector3.Distance(transform.position, target.position) <= minimumDistance + 0.1f)
                        {
                            StartCoroutine(PerformMeleeAttack());
                            yield return new WaitForSeconds(1.1f);
                            interruptMovement = false;
                        }
                        else
                        {
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    break;
                case 2:
                    isLeaping = true;
                    StartCoroutine(PerformLeapAttack());
                    yield return new WaitForSeconds(leapCooldown);
                    interruptMovement = false;
                    break;
            }
            attackLock = false;
        }
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator PhaseFour()
    {
        while (startPhaseFour)
        {
            if (isSetup)
            {
                if (!attackLock)
                {
                    random = rnd.Next(0, 3);
                    Debug.Log(random);
                }
                attackLock = true;
                yield return new WaitForSeconds(1f);
                switch (random)
                {
                    case 0:
                        isStomping = true;
                        StartCoroutine(PerformStompAttack());
                        yield return new WaitForSeconds(3f);
                        interruptMovement = false;
                        break;
                    case 1:
                        for (int i = 0; i < 20; i++)
                        {
                            if (Vector3.Distance(transform.position, target.position) <= minimumDistance + 0.1f)
                            {
                                StartCoroutine(PerformMeleeAttack());
                                yield return new WaitForSeconds(1.1f);
                                interruptMovement = false;
                            }
                            else
                            {
                                yield return new WaitForSeconds(1.1f);
                            }
                        }
                        break;
                    case 2:
                        isLeaping = true;
                        StartCoroutine(PerformLeapAttack());
                        yield return new WaitForSeconds(leapCooldown);
                        interruptMovement = false;
                        break;
                }
                attackLock = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator PerformMeleeAttack()
    {
        interruptMovement = true;
        animator.SetTrigger("Slam");
        yield return new WaitForSeconds(0.5f);

        PlayAttackSound(0); // Play melee sound (index 0)

        if (attackPrefabs.Length > 0)
        {
            Instantiate(attackPrefabs[0], transform.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(0.25f);
    }

    private IEnumerator PerformLeapAttack()
    {
        while (isLeaping)
        {
            if (!proLock)
            {
                proLock = true;
                interruptMovement = true;
                animator.SetTrigger("Leap");
                yield return new WaitForSeconds(0.66f);

                Vector3 leapDirection = (target.position - transform.position).normalized;
                Vector3 dir = leapDirection;
                dir.y = 0;
                leapDirection = dir;
                leapDirection *= ((Math.Abs(leapDirection.z) * .6f) + 1);
                float elapsedTime = 0f;
                FlipSprite(leapDirection.x);
                Vector3 position = transform.position;

                while (elapsedTime < 0.42f && !stopLeap)
                {
                    position = transform.position;
                    position.y = 1;
                    transform.position = position;
                    transform.position += leapDirection * 15 * Time.deltaTime;
                    FlipSprite(leapDirection.x);
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForSeconds(0.001f);
                }
                position.y = HEIGHT;
                transform.position = position;

                Instantiate(attackPrefabs[1], transform.position, Quaternion.identity);
                isLeaping = false;
                proLock = false;
            }
            PlayAttackSound(0); // Play leap sound (index 0)
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator PerformStompAttack()
    {
        int arcAngle = 360;
        float startAngle = -arcAngle / 2f;

        while (isStomping)
        {
            if (!proLock)
            {
                proLock = true;
                interruptMovement = true;
                animator.SetTrigger("Stomp");
                yield return new WaitForSeconds(0.75f);

                Instantiate(attackPrefabs[2], target.position, Quaternion.identity);
                for (int i = 1; i <= 3; i++)
                {
                    yield return new WaitForSeconds(0.2f);

                    PlayAttackSound(1);

                    for (int j = 0; j < (i * 4); j++)
                    {
                        Vector3 directionToTarget = (target.position - transform.position).normalized;
                        FlipSprite(directionToTarget.x);
                        float angle = startAngle + (arcAngle / ((i * 4)) * j);
                        Vector3 spreadDirection = Quaternion.Euler(0, angle, 0) * directionToTarget;

                        Instantiate(attackPrefabs[2], (target.position + (spreadDirection * i)), Quaternion.identity);
                    } 
                    yield return new WaitForSeconds(0.2f);
                    StartCoroutine(DelayedPlayAttackSound(2, 0.4f));
                }
                interruptMovement = false;
                isStomping = false;
                proLock = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator SetupArena()
    {
        boardSwitcher.focusOnBoss = true;
        speed += 1;
        PlayerController[] playerController = FindObjectsOfType<PlayerController>();
        SinglePlayerAttack[] singlePlayerAttacks = FindObjectsOfType<SinglePlayerAttack>();
        foreach (PlayerController player in playerController)
            player.LockMovement(10f);
        foreach (SinglePlayerAttack singlePlayerAttack in singlePlayerAttacks)
            singlePlayerAttack.attackChecker = false;

        // Move to waypoint
        while (Vector3.Distance(transform.position, returnWaypoint.position) > 0.5f)
        {
            MoveTowardsWapoint();
            yield return new WaitForSeconds(0.01f);
        }

        // Pull player and trap them
        StartCoroutine(PullPlayerAndTrap());
    }

    private IEnumerator PullPlayerAndTrap()
    {
        PlayerController[] playerController = FindObjectsOfType<PlayerController>();
        SinglePlayerAttack[] singlePlayerAttacks = FindObjectsOfType<SinglePlayerAttack>();
        StopCoroutine(SetupArena());
        
        interruptMovement = true;
        animator.SetTrigger("Raise");
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i < 3; i++)
        {

            foreach (PlayerController player in playerController)
                player.transform.position = playerWaypoint.position;
            yield return new WaitForSeconds(0.01f);
        }

        Instantiate(attackPrefabs[3], targetWaypoint.position + new Vector3(0, -1f, 0), new Quaternion(0, 0, 0, 0));
        PlayAttackSound(3); // Play build arena sound (index 3)

        yield return new WaitForSeconds(1f);
        boardSwitcher.focusOnBoss = false;
        foreach (PlayerController player in playerController)
            player.UnlockMovement();
        yield return new WaitForSeconds(2f);
        speed -= 1;
        interruptMovement = false;
        isSetup=true;
        foreach (SinglePlayerAttack singlePlayerAttack in singlePlayerAttacks)
            singlePlayerAttack.attackChecker = true;
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
            stopLeap = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        stopLeap = false;
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
            stopLeap = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        stopLeap = false;
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
            sfxAudioSource.PlayOneShot(attackSounds[soundIndex]);
        }
        else
        {
            Debug.LogWarning($"Attack sound at index {soundIndex} is not assigned!");
        }
    }

    public IEnumerator DelayedPlayAttackSound(int soundIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayAttackSound(soundIndex);
    }
}