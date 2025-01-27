using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform player;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float health = 100f;
    public Slider healthBar;

    private int currentWaypointIndex = 0;
    private bool isChasing = false;

    private void Start()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = health;
            healthBar.value = health;
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            EnableOutline(true); // Turn on red outline
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            EnableOutline(false); // Turn off red outline
            Patrol();
        }
    }

    private void Patrol()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        transform.Translate(direction * patrolSpeed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * chaseSpeed * Time.deltaTime, Space.World);
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
