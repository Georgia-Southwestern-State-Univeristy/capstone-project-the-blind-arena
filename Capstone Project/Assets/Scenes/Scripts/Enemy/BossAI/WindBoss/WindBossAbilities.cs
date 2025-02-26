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
    public float retreatDistance = 5f; // Distance to maintain from player
    public float pushDistance = 2f; // Distance at which push activates
    public Transform target;
    public Animator animator;
    public GameObject[] attackPrefabs;

    private EnemyHealth enemyHealth;
    private bool isRetreating;
    private bool isShooting;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (!enemyHealth) Debug.LogError("EnemyHealth component not found!");

        StartCoroutine(ProjectileAttackLoop());
    }

    private void Update()
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

            // Apply force only in X and Z directions
            player.ApplyExternalForce(pushVelocity, 0.5f);
        }
    }


    private IEnumerator ProjectileAttackLoop()
    {
        while (true)
        {
            if (attackPrefabs.Length > 0)
            {
                Instantiate(attackPrefabs[Random.Range(0, attackPrefabs.Length)], transform.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(projectileAttackRate);
        }
    }

    private void FlipSprite(float directionX)
    {
        transform.localScale = new Vector3(Mathf.Sign(directionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
