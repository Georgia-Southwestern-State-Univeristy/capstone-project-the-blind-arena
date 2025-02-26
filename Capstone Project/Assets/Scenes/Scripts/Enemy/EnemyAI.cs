using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform player;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float health = 100f;
    public Slider healthBar;

    private int currentWaypointIndex = 0;
    private bool isChasing = false;
    private bool isAttacking = false;
    private Animator animator;
    private void Start()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health;
        }

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(AttackPlayer());
        }
        else if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            Patrol();
        }
    }

    private void Patrol()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        Vector3 movement = direction * patrolSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        animator.SetFloat("speed", movement.magnitude);

        FlipSprite(direction.x);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 movement = direction * chaseSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        animator.SetFloat("speed", movement.magnitude);

        FlipSprite(direction.x);
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        if (animator != null) animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackCooldown);

        if (animator != null) animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private void FlipSprite(float directionX)
    {
        if (directionX < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (directionX > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (healthBar != null)
        {
            healthBar.value = health;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
