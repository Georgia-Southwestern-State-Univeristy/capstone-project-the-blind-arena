using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBossAI : MonoBehaviour
{
    public float speed = 5f;
    public Transform target;
    public float minimumDistance = 5f;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public float bubbleSpawnRate = 5f;
    public float bubbleSpawnRadius = 3f;
    public float tsunamiDamage = 25f;
    public Animator animator;
    public GameObject waterWhipPrefab;
    public GameObject waterBubblePrefab;
    public GameObject iceSpikePrefab;
    public GameObject tsunamiPrefab;
    public Transform tsunamiSpawnPoint;

    private EnemyHealth enemyHealth;
    private bool isAttacking = false;
    private bool hasEnteredDesperation = false;
    private float lastBubbleSpawnTime;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        lastBubbleSpawnTime = Time.time;
    }

    private void Update()
    {
        float healthPercentage = (float)enemyHealth.currentHealth / enemyHealth.maxHealth * 100f;

        // Tsunami phase at 25% health
        if (healthPercentage <= 25f && !hasEnteredDesperation)
        {
            hasEnteredDesperation = true;
            StartCoroutine(TsunamiBarrage());
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // Attack pattern based on distance
        if (!isAttacking)
        {
            if (distanceToPlayer <= minimumDistance)
            {
                StartCoroutine(WaterWhipAttack());
            }
            else
            {
                StartCoroutine(IceSpikeAttack());
            }
        }

        // Spawn water bubbles periodically
        if (Time.time >= lastBubbleSpawnTime + bubbleSpawnRate)
        {
            SpawnWaterBubble();
            lastBubbleSpawnTime = Time.time;
        }

        MoveTowardsPlayer();
        FlipSprite(target.position.x - transform.position.x);
    }

    private void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer > minimumDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private IEnumerator WaterWhipAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("WaterWhip");

        GameObject whip = Instantiate(waterWhipPrefab, transform.position, Quaternion.identity);
        whip.transform.parent = transform;

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    private IEnumerator IceSpikeAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("IceSpike");

        Vector3 direction = (target.position - transform.position).normalized;
        GameObject spike = Instantiate(iceSpikePrefab, transform.position, Quaternion.LookRotation(direction));

        yield return new WaitForSeconds(projectileAttackRate);
        isAttacking = false;
    }

    private void SpawnWaterBubble()
    {
        // Calculate random position near the player
        Vector2 randomCircle = Random.insideUnitCircle * bubbleSpawnRadius;
        Vector3 spawnPosition = target.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        GameObject bubble = Instantiate(waterBubblePrefab, spawnPosition, Quaternion.identity);
        Destroy(bubble, 5f); // Destroy bubble after 5 seconds if not interacted with
    }

    private IEnumerator TsunamiBarrage()
    {
        if (animator != null)
            animator.SetTrigger("Tsunami");

        GameObject tsunami = Instantiate(tsunamiPrefab, tsunamiSpawnPoint.position, Quaternion.identity);

        // Configure tsunami properties
        Projectile tsunamiProjectile = tsunami.GetComponent<Projectile>();
        if (tsunamiProjectile != null)
        {
            tsunamiProjectile.speed = 15f;
        }

        yield return new WaitForSeconds(attackDelay * 2f); // Longer delay for tsunami
    }
}
