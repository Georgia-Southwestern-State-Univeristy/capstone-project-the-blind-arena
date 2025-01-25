using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] waypoints; // Patrol points
    public Transform player; // Reference to the player
    public float detectionRange = 10f; // Range to detect the player
    public float attackRange = 2f; // Range to attack the player
    public float attackCooldown = 2f; // Time between attacks
    public int damage = 10; // Damage to the player

    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private float lastAttackTime = 0;

    private enum EnemyState { Idle, Patrol, Chase, Attack }
    private EnemyState currentState = EnemyState.Patrol;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolBehavior(distanceToPlayer);
                break;

            case EnemyState.Chase:
                ChaseBehavior(distanceToPlayer);
                break;

            case EnemyState.Attack:
                AttackBehavior(distanceToPlayer);
                break;

            case EnemyState.Idle:
                IdleBehavior();
                break;
        }
    }

    private void PatrolBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer < detectionRange)
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (agent.remainingDistance < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void ChaseBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer > detectionRange)
        {
            currentState = EnemyState.Patrol;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            return;
        }

        agent.SetDestination(player.position);
    }

    private void AttackBehavior(float distanceToPlayer)
    {
        agent.SetDestination(transform.position); // Stop moving

        if (distanceToPlayer > attackRange)
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            AttackPlayer();
        }
    }

    private void IdleBehavior()
    {
        // Perform idle animations or behaviors
    }

    private void AttackPlayer()
    {
        Debug.Log("Enemy attacked the player!");
        // Add player damage logic here
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection and attack ranges in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}