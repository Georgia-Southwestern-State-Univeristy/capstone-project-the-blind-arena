using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private Transform[] patrolPoints;

    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private bool isKnockedBack = false;
    private Rigidbody rb;
    private Vector3 targetPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (patrolPoints.Length > 0)
            targetPosition = patrolPoints[currentPatrolIndex].position;
    }

    private void Update()
    {
        if (isKnockedBack) return; // Stop movement during knockback

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isChasing = distanceToPlayer < detectionRange || (isChasing && distanceToPlayer <= detectionRange * 1.2f);

        targetPosition = isChasing ? player.position : patrolPoints[currentPatrolIndex].position;
        MoveTowardsTarget(isChasing ? chaseSpeed : patrolSpeed);
    }

    private void MoveTowardsTarget(float speed)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Ensure movement stays on the XZ plane
        rb.linearVelocity = direction * speed;

        if (!isChasing && Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            targetPosition = patrolPoints[currentPatrolIndex].position;
        }
    }

    public void DisableMovement(float duration)
    {
        StartCoroutine(DisableMovementCoroutine(duration));
    }

    private IEnumerator DisableMovementCoroutine(float duration)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector3.zero; // Stop movement immediately
        yield return new WaitForSeconds(duration);
        isKnockedBack = false;
    }
}
