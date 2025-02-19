using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBossAI : MonoBehaviour
{
    public float speed;
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public float leapCooldown = 5f;
    public float pullCooldown = 8f;
    public float meleeRange = 2f;
    public Animator animator;
    public GameObject[] attackPrefabs;
    public GameObject terrainBlockPrefab;  // Blocking terrain object
    public Transform spikeSpawnPoint;

    private EnemyHealth enemyHealth;
    private bool isReturning = false;
    private bool hasUsedSecondAbility = false;
    private bool hasEnteredShootAndRetreat = false;
    private bool canLeap = true;
    private bool canPull = true;

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
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth / 2 && !isReturning)
        {
            isReturning = true;
        }

        if (enemyHealth.currentHealth <= enemyHealth.maxHealth / 4 && !hasEnteredShootAndRetreat)
        {
            hasEnteredShootAndRetreat = true;
        }

        if (!isReturning)
        {
            if (Vector3.Distance(transform.position, target.position) <= meleeRange)
            {
                StartCoroutine(PerformMeleeAttack());
            }
            else
            {
                MoveTowardsPlayer();
            }

            if (canLeap)
            {
                StartCoroutine(LeapToPlayer());
            }

            if (canPull)
            {
                StartCoroutine(PullPlayerAndTrap());
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 movement = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.position = movement;
        animator.SetFloat("speed", movement.magnitude);
        FlipSprite(direction.x);
    }

    private void ReturnToWaypoint()
    {
        if (Vector3.Distance(transform.position, returnWaypoint.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, returnWaypoint.position, speed * Time.deltaTime);
        }
        else if (!hasUsedSecondAbility)
        {
            StartCoroutine(UseSecondAbility());
        }
    }

    private IEnumerator UseSecondAbility()
    {
        hasUsedSecondAbility = true;
        if (animator != null) animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);

        if (attackPrefabs.Length > 1)
        {
            Instantiate(attackPrefabs[1], transform.position, Quaternion.identity);
        }

        isReturning = false;
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
        canLeap = false;
        yield return new WaitForSeconds(leapCooldown);

        if (animator != null)
        {
            animator.SetTrigger("Leap");
        }

        Vector3 jumpTarget = target.position;
        transform.position = jumpTarget;

        yield return new WaitForSeconds(0.5f);
        if (attackPrefabs.Length > 2)
        {
            Instantiate(attackPrefabs[2], transform.position, Quaternion.identity);
        }

        canLeap = true;
    }

    private IEnumerator PullPlayerAndTrap()
    {
        canPull = false;
        yield return new WaitForSeconds(pullCooldown);

        if (animator != null)
        {
            animator.SetTrigger("Pull");
        }

        target.position = transform.position;

        yield return new WaitForSeconds(0.5f);

        Instantiate(terrainBlockPrefab, transform.position + new Vector3(3, 0, 0), Quaternion.identity);
        Instantiate(terrainBlockPrefab, transform.position + new Vector3(-3, 0, 0), Quaternion.identity);
        Instantiate(terrainBlockPrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        Instantiate(terrainBlockPrefab, transform.position + new Vector3(0, -3, 0), Quaternion.identity);

        canPull = true;
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
