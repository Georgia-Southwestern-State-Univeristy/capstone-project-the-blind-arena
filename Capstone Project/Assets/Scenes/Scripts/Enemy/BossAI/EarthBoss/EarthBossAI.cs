using System;
using System.Collections;
using UnityEngine;

public class EarthBossAI : MonoBehaviour
{
    public float speed;
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance = 2f;
    public float attackDelay = 2f;
    public float leapCooldown = 5f;
    public float spikeCooldown = 3f;
    public float pullCooldown = 8f;
    public Animator animator;
    public GameObject[] attackPrefabs;
    public Transform spikeSpawnPoint;

    private EnemyHealth enemyHealth;
    private float initialSpeed;
    private bool startPhaseOne = false;
    private bool startPhaseTwo = false;
    private bool startPhaseThree = false;
    private bool startPhaseFour = false;
    private bool isSetup = false;
    private bool isLeaping = false;
    private bool isSpiking = false;
    private bool isFocused = false;
    private bool interruptMovement;
    private bool attackLock, leapLock, genLock, proLock;
    private bool targetLock = false;
    private bool p1, p2, p3;
    private const float HEIGHT = 0.6f;
    private System.Random rnd = new System.Random();
    private int random;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        Rigidbody rb = GetComponent<Rigidbody>();
        initialSpeed = speed;

        if (!enemyHealth) Debug.LogError("EnemyHealth component not found!");
    }

    private void Update()
    {
        float healthPercentage = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;

        if (!targetLock)
        {
            targetLock = true;
            StartCoroutine(CheckForTarget());
        }

        if (healthPercentage <= 0.25f)
        {
            // Phase 4: Move to waypoint and start pull/trap sequence
            if (!startPhaseFour)
            {
                startPhaseFour = true;
                StopAllCoroutines();
                interruptMovement = false;
                attackLock = false;
                StartCoroutine(PhaseFour());
            }
            if (Vector3.Distance(transform.position, target.position) > minimumDistance && !interruptMovement && isSetup)
            {
                MoveTowardsPlayer();
            }
            else if (!isSetup)
            {
                MoveTowardsWapoint();
                if (Vector3.Distance(transform.position, returnWaypoint.position) <= 1f)
                {
                    
                }
            }
            else
            {
                animator.SetFloat("speed", 0);
            }
            return;
        }
        else if (healthPercentage <= 0.50f)
        {
            // Phase 3: Disable leaping, enable spike attack
            if (!startPhaseThree)
            {
                startPhaseThree = true;
                StopAllCoroutines();
                interruptMovement = false;
                attackLock = false;
                StartCoroutine(PhaseThree());
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
        else if (healthPercentage <= 0.75f)
        {
            // Phase 2: Enable Leaping, more aggression
            if (!startPhaseTwo)
            {
                startPhaseTwo = true;
                StopAllCoroutines();
                interruptMovement = false;
                attackLock = false;
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
            // Phase 1: Melee Combat
            if (!startPhaseOne)
            {
                startPhaseOne = true;
                interruptMovement = false;
                attackLock = false;
                StartCoroutine(PhaseOne());
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

        }
        yield return null;
    }

    private IEnumerator PhaseTwo()
    {
        while (startPhaseTwo)
        {

        }
        yield return null;
    }

    private IEnumerator PhaseThree()
    {
        while (startPhaseThree)
        {

        }
        yield return null;
    }

    private IEnumerator PhaseFour()
    {
        while (startPhaseFour)
        {

        }
        yield return null;
    }

    private IEnumerator PerformMeleeAttack()
    {
        animator.SetTrigger("MeleeAttack");
        yield return new WaitForSeconds(attackDelay);

        if (attackPrefabs.Length > 0)
        {
            Instantiate(attackPrefabs[0], transform.position, Quaternion.identity);
        }
    }

    private IEnumerator LeapToPlayer()
    {
        while (isLeaping)
        {
            yield return new WaitForSeconds(leapCooldown);

            animator.SetTrigger("Leap");
            transform.position = target.position; // Teleporting for simplicity; use an animation for better effect.

            yield return new WaitForSeconds(0.5f);
            if (attackPrefabs.Length > 2)
            {
                Instantiate(attackPrefabs[2], transform.position, Quaternion.identity);
            }
        }
    }

    private IEnumerator ShootEarthSpikes()
    {
        while (isSpiking)
        {
            yield return new WaitForSeconds(spikeCooldown);

            animator.SetTrigger("ShootSpikes");
            Instantiate(attackPrefabs[0], spikeSpawnPoint.position, Quaternion.identity);
        }
    }

    private IEnumerator ReturnAndTrapPlayer()
    {
        // Move to waypoint
        while (Vector3.Distance(transform.position, returnWaypoint.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, returnWaypoint.position, speed * Time.deltaTime);
            yield return null;
        }

        // Pull player and trap them
        StartCoroutine(PullPlayerAndTrap());
    }

    private void ToggleBooleans()
    {
        interruptMovement = false;
        attackLock = false;
        genLock = false;
    }

    private IEnumerator PullPlayerAndTrap()
    {
        animator.SetTrigger("Pull");
        yield return new WaitForSeconds(0.5f);
        target.position = transform.position;

        Instantiate(attackPrefabs[0], transform.position + new Vector3(3, 0, 0), Quaternion.identity);
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}