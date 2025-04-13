using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatState : FireBossState
{
    private bool isReturning = true; // Keeps track of the retreat state
    private Vector3 targetPosition;
    private float retreatSpeed = 5f;
    private float fixedHeight = 0.6f;

    public RetreatState(FireBossStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        // Make sure the returnWaypoint is assigned
        if (stateMachine.returnWaypoint != null)
        {
            targetPosition = stateMachine.returnWaypoint.position;
            Debug.Log("Boss is retreating to return waypoint: " + targetPosition);
        }
        else
        {
            Debug.LogError("No returnWaypoint assigned!");
        }

        // Start the retreating logic (if needed)
        stateMachine.StartCoroutine(RetreatLogic());
    }

    public override void UpdateState()
    {
        // This can stay empty, as the logic is handled within the coroutine (RetreatLogic)
    }

    public override void ExitState()
    {
        // Any cleanup needed when exiting the retreat state
        isReturning = false;
        Debug.Log("Boss finished retreating!");
    }

    private IEnumerator RetreatLogic()
    {
        while (isReturning && stateMachine.enemyHealth.currentHealth <= stateMachine.enemyHealth.maxHealth * 0.5f && stateMachine.enemyHealth.currentHealth > stateMachine.enemyHealth.maxHealth * 0.25f)
        {
            // Move toward the return waypoint
            Vector3 directionToWaypoint = (stateMachine.returnWaypoint.position - stateMachine.transform.position).normalized;
            stateMachine.FlipSprite(directionToWaypoint.x); // Flip the sprite based on direction

            // Move towards the waypoint
            stateMachine.transform.position += directionToWaypoint * retreatSpeed * Time.deltaTime;

            // If we reached the waypoint, shoot projectiles in an arc
            if (Vector3.Distance(stateMachine.transform.position, stateMachine.returnWaypoint.position) < 0.5f)
            {
                ShootProjectilesInArc(8, 120f);  // Assuming you have this method to shoot projectiles in an arc
                stateMachine.FlipSprite(stateMachine.target.position.x);

                // Wait a bit before next volley
                yield return new WaitForSeconds(2f);

                // Move to a new random position around the waypoint
                Vector3 randomOffset = Random.insideUnitSphere * 3f;
                randomOffset.y = 0; // Keep on same Y plane
                Vector3 newPosition = stateMachine.returnWaypoint.position + randomOffset;

                // Update boss Y position for consistent height
                Vector3 position = stateMachine.transform.position;
                position.y = fixedHeight;
                stateMachine.transform.position = position;

                // Move to new position
                while (Vector3.Distance(stateMachine.transform.position, newPosition) > 0.5f && isReturning)
                {
                    stateMachine.transform.position = Vector3.MoveTowards(stateMachine.transform.position, newPosition, retreatSpeed * Time.deltaTime);
                    yield return null;
                }
            }

            yield return null;
        }

        // After the retreat process is done, transition back to IdleState
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    private void ShootProjectilesInArc(int numberOfProjectiles, float arcAngle)
    {
        // Implement the shooting logic based on your projectiles
        // This method assumes you're shooting projectiles in an arc around the boss
        Debug.Log("Shooting projectiles in an arc!");
    }
}
