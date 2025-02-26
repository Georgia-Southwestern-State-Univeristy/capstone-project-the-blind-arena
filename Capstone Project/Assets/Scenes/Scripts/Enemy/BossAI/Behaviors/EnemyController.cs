using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance;
    public Animator animator;

    private bool isReturning;

    public void MoveTowardsPlayer()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - target.position.magnitude));
        FlipSprite(direction.x);
    }

    public void ReturnToWaypoint()
    {
        if (returnWaypoint == null) return;

        if (Vector3.Distance(transform.position, returnWaypoint.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, returnWaypoint.position, speed * Time.deltaTime);
        }
        else
        {
            isReturning = false;
        }
    }

    private void FlipSprite(float directionX)
    {
        transform.localScale = new Vector3(Mathf.Sign(directionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
