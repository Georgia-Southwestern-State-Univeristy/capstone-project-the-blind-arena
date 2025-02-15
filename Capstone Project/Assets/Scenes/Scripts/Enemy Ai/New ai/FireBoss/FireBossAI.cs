using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBossAI : MonoBehaviour
{
    public float speed;
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance;
    public float attackDelay = 2f;
    public Animator animator;
    public GameObject[] attackPrefabs;

    private EnemyHealth enemyHealth;
    private Shootandretreat shootAndRetreat; // Reference to Shootandretreat script
    private bool isAttacking = false;
    private bool isReturning = false;
    private bool hasUsedSecondAbility = false;
    private bool hasEnteredShootAndRetreat = false;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        shootAndRetreat = GetComponent<Shootandretreat>(); // Get the Shootandretreat script
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth component not found!");
        }
        if (shootAndRetreat == null)
        {
            Debug.LogError("Shootandretreat component not found!");
        }
    }

    private void Update()
    {
        if (hasEnteredShootAndRetreat)
        {
            shootAndRetreat.enabled = true; // Enable Shootandretreat behavior
        }
        else if (isReturning)
        {
            ReturnToWaypoint();
        }
        else if (Vector3.Distance(transform.position, target.position) > minimumDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else if (!isAttacking)
        {
            StartCoroutine(AttackSequence());
        }

        // Check health for 50% and 25% thresholds
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth / 2 && !isReturning)
        {
            isReturning = true; // Start returning to waypoint at 50% health
        }

        if (enemyHealth.currentHealth <= enemyHealth.maxHealth / 4 && !hasEnteredShootAndRetreat)
        {
            hasEnteredShootAndRetreat = true; // Start shoot-and-retreat behavior at 25% health
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(attackDelay);

        if (attackPrefabs.Length > 0)
        {
            int attackIndex = Random.Range(0, attackPrefabs.Length);
            Instantiate(attackPrefabs[attackIndex], transform.position, Quaternion.identity);
        }

        isAttacking = false;
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

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(attackDelay);

        if (attackPrefabs.Length > 1)
        {
            Instantiate(attackPrefabs[1], transform.position, Quaternion.identity);
        }

        isReturning = false;
    }
}


