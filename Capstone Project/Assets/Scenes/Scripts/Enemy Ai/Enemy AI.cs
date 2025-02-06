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
        if (isAttacking) {
            animator.SetFloat("Speed", 0f); // Stop movement animation during attack
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(AttackPlayer()); // Start attack sequence
        }
        else if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            EnableOutline(true);
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            EnableOutline(false);
            Patrol();
        }
    }

    private void Patrol()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        Vector3 movement = direction * patrolSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        animator.SetFloat("Speed", movement.magnitude / Time.deltaTime); // Update speed for animation

        bool movingLeft = movement.x < 0;
        bool movingRight = movement.x > 0;

        if (movingLeft)
        {
            // Flip the player without affecting the Y or Z scale
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        if (movingRight)
        {
            // Ensure the player faces right without affecting the Y or Z scale
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 movement = direction * patrolSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        animator.SetFloat("Speed", movement.magnitude / Time.deltaTime); // Update speed for animation

        bool movingLeft = movement.x < 0;
        bool movingRight = movement.x > 0;

        if (movingLeft)
        {
            // Flip the player without affecting the Y or Z scale
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        if (movingRight)
        {
            // Ensure the player faces right without affecting the Y or Z scale
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        if (animator != null) animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackCooldown); // Wait for attack animation to finish

        if (animator != null) animator.SetBool("isAttacking", false);
        isAttacking = false; // Allow AI to resume movement
    }

    private void EnableOutline(bool enable)
    {
        var outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = enable;
        }
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