using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public Transform target;
    public Transform returnWaypoint;
    public Animator animator;

    private bool isDashing = false;
    private bool isReturning = false;
    private bool isMoving = true;
    private const float HEIGHT = 0.6f;

    private void Update()
    {
        if (!isMoving) return;

        if (isReturning)
        {
            ReturnToWaypoint();
        }
        else if (!isDashing && target != null && Vector3.Distance(transform.position, target.position) > 1f)
        {
            MoveTowardsTarget();
        }
        else
        {
            animator.SetFloat("speed", 0);
        }
    }

    public void MoveTowardsTarget()
    {
        if (target == null || isDashing) return; // Prevent movement during dash

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        animator.SetFloat("speed", Mathf.Abs(direction.magnitude));
        FlipSprite(direction.x);
        Vector3 position = transform.position;
        position.y = HEIGHT;
        transform.position = position;
    }

    public IEnumerator DashAttack()
    {
        if (isDashing) yield break;

        isDashing = true;
        isMoving = false;
        animator.SetTrigger("Dash");

        yield return new WaitForSeconds(0.5f);  // Wind-up delay

        Vector3 dashDirection = (target.position - transform.position).normalized;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            if (!gameObject.activeInHierarchy) yield break; // Stop if object is disabled

            transform.position += dashDirection * dashSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);  // Cooldown after dash

        isDashing = false;
        isMoving = true; //  Resume movement after dash
    }

    public void ReturnToWaypoint()
    {
        if (returnWaypoint == null) return;

        isReturning = true;
        transform.position = Vector3.MoveTowards(transform.position, returnWaypoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, returnWaypoint.position) < 0.5f)
        {
            isReturning = false; // Stop returning once reached
        }
    }

    private void FlipSprite(float directionX)
    {
        transform.localScale = new Vector3(Mathf.Sign(directionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public bool IsDashing => isDashing;
    public bool IsReturning { get => isReturning; set => isReturning = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }
}
