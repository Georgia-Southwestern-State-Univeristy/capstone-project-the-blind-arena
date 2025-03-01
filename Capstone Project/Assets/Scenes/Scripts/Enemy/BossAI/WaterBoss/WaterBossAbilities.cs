using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBossAI : MonoBehaviour
{
    public float speed;
    public Transform target;
    public float minimumDistance;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public Animator animator;
    public GameObject waterWhipPrefab;
    public GameObject waterBubblePrefab;
    public GameObject iceSpikePrefab;
    public GameObject tsunamiPrefab;
    public Transform tsunamiSpawnPoint;

    private EnemyHealth enemyHealth;
    private bool isAttacking = false;
    private bool hasEnteredDesperation = false;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth / 4 && !hasEnteredDesperation)
        {
            hasEnteredDesperation = true;
            StartCoroutine(TsunamiBarrage());
        }
        else if (Vector3.Distance(transform.position, target.position) <= minimumDistance)
        {
            StartCoroutine(WaterWhipAttack());
        }
        else
        {
            StartCoroutine(IceSpikeAttack());
        }

        SpawnWaterBubble();
        MoveTowardsPlayer();
        FlipSprite(target.position.x - transform.position.x);
    }

    private void MoveTowardsPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) > minimumDistance)
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
        Instantiate(waterWhipPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    private IEnumerator IceSpikeAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        Instantiate(iceSpikePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(projectileAttackRate);
        isAttacking = false;
    }

    private void SpawnWaterBubble()
    {
        if (Random.Range(0, 100) < 5)
        {
            Vector3 spawnPosition = target.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            Instantiate(waterBubblePrefab, spawnPosition, Quaternion.identity);
        }
    }

    private IEnumerator TsunamiBarrage()
    {
        Instantiate(tsunamiPrefab, tsunamiSpawnPoint.position, Quaternion.identity);
        yield return new WaitForSeconds(attackDelay);
    }
}
