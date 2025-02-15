using System.Collections;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public float patrolSpeed;
    public Transform[] waypoints;
    public float waitTime;
    private int currentWaypointIndex;
    private bool isWaiting;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isWaiting)
        {
            MoveToWaypoint();
        }
    }

    private void MoveToWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        Vector3 movement = direction * patrolSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        animator.SetFloat("Speed", movement.magnitude / Time.deltaTime);
        FlipSprite(direction.x);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetFloat("Speed", 0);
        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isWaiting = false;
    }

    private void FlipSprite(float directionX)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX > 0 ? 1 : -1);
        transform.localScale = scale;
    }
}

