using System.Collections;
using UnityEngine;

public class NewFireBossAI : MonoBehaviour
{
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public float minimumDistance = 5f;
    public GameObject[] attackPrefabs;

    private EnemyController enemyController;
    private EnemyHealth enemyHealth;
    private ShootAndRetreat shootAndRetreat;
    private bool hasEnteredShootAndRetreat = false;
    private bool isThrowingProjectiles = false;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyHealth = GetComponent<EnemyHealth>();
        shootAndRetreat = GetComponent<ShootAndRetreat>();

        if (!enemyController) Debug.LogError("EnemyController component not found!");
        if (!enemyHealth) Debug.LogError("EnemyHealth component not found!");
        if (!shootAndRetreat) Debug.LogError("ShootAndRetreat component not found!");
    }

    private void Update()
    {
        if (hasEnteredShootAndRetreat)
        {
            shootAndRetreat.enabled = true;
            enemyController.IsMoving = false;
            return;
        }

        if (enemyController.IsReturning)
        {
            enemyController.ReturnToWaypoint();
            return;
        }

        if (!enemyController.IsDashing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, enemyController.target.position);

            if (distanceToPlayer > minimumDistance)
            {
                enemyController.IsMoving = true;
            }
            else
            {
                enemyController.IsMoving = false;
                StartCoroutine(DashThenResume());
            }
        }

        if (enemyHealth.currentHealth <= enemyHealth.maxHealth / 2 && !enemyController.IsReturning)
        {
            enemyController.IsReturning = true;
        }

        if (enemyHealth.currentHealth <= enemyHealth.maxHealth / 4 && !hasEnteredShootAndRetreat)
        {
            hasEnteredShootAndRetreat = true;
        }
    }

    private IEnumerator DashThenResume()
    {
        yield return StartCoroutine(enemyController.DashAttack());

        // Resume movement after dash attack ends
        enemyController.IsMoving = true;

        StartCoroutine(ProjectileAttackLoop());
    }

    private IEnumerator ProjectileAttackLoop()
    {
        isThrowingProjectiles = true;

        while (isThrowingProjectiles)
        {
            if (attackPrefabs.Length > 0)
            {
                Debug.Log("Throwing projectile...");
                Instantiate(attackPrefabs[Random.Range(0, attackPrefabs.Length)], transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("No attackPrefabs assigned!");
            }

            yield return new WaitForSeconds(projectileAttackRate);

            if (Vector3.Distance(transform.position, enemyController.target.position) <= minimumDistance)
            {
                isThrowingProjectiles = false;
                StartCoroutine(DashThenResume());
                yield break;
            }
        }
    }
}
