using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBossAI : MonoBehaviour
{
    public float speed;
    public Transform target;
    public Transform returnWaypoint;
    public float meleeRange = 2f;
    public float attackDelay = 2f;
    public float leapCooldown = 5f;
    public float spikeCooldown = 3f;
    public float pullCooldown = 8f;
    public Animator animator;
    public GameObject[] attackPrefabs;
    public GameObject terrainBlockPrefab; // Blocking terrain object
    public Transform spikeSpawnPoint;

    private EnemyHealth enemyHealth;
    private bool canLeap = false;
    private bool canShootSpikes = false;
    private bool canPull = false;
    private bool isReturning = false;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();

        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth component not found!");
        }
    }

    private void Update()
    {
        float healthPercentage = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;

        if (healthPercentage > 0.75f)
        {
            // Phase 1: Normal melee combat
            if (Vector3.Distance(transform.position, target.position) <= meleeRange)
            {
                StartCoroutine(PerformMeleeAttack());
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
        else if (healthPercentage > 0.50f)
        {
            // Phase 2: Enable leaping
            if (!canLeap)
            {
                canLeap = true;
                StartCoroutine(LeapToPlayer());
            }
        }
        else if (healthPercentage > 0.25f)
        {
            // Phase 3: Disable leaping, enable spike shooting
            canLeap = false;
            if (!canShootSpikes)
            {
                canShootSpikes = true;
                StartCoroutine(ShootEarthSpikes());
            }
        }
        else
        {
            // Phase 4: Move to waypoint and start pull/trap sequence
            if (!isReturning)
            {
                isReturning = true;
                StartCoroutine(ReturnAndTrapPlayer());
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        animator.SetFloat("speed", speed);
        FlipSprite(direction.x);
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
        while (canLeap)
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
        while (canShootSpikes)
        {
            yield return new WaitForSeconds(spikeCooldown);

            animator.SetTrigger("ShootSpikes");
            Instantiate(attackPrefabs[1], spikeSpawnPoint.position, Quaternion.identity);
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

    private IEnumerator PullPlayerAndTrap()
    {
        animator.SetTrigger("Pull");
        yield return new WaitForSeconds(0.5f);
        target.position = transform.position;

        Instantiate(terrainBlockPrefab, transform.position + new Vector3(3, 0, 0), Quaternion.identity);
        Instantiate(terrainBlockPrefab, transform.position + new Vector3(-3, 0, 0), Quaternion.identity);
        Instantiate(terrainBlockPrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        Instantiate(terrainBlockPrefab, transform.position + new Vector3(0, -3, 0), Quaternion.identity);
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}