using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyChaseState : EnemyState
{

    private Transform _playerTransform;
    private float _movementSpeed = 1.75f;
    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();

        Debug.Log("Hello from the cahse state");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (enemy.IsAggroed)
        {
            // Calculate movement direction, only affecting X and Z axes
            Vector3 moveDirection = new Vector3(
                _playerTransform.position.x - enemy.transform.position.x,
                0f, // Ignore Y-axis movement
                _playerTransform.position.z - enemy.transform.position.z
            ).normalized;

            // Apply movement using Rigidbody.MovePosition (better for physics-based movement)
            Rigidbody rb = enemy.GetComponent<Rigidbody>();

            // Ensure Rigidbody exists before applying movement
            if (rb != null)
            {
                rb.MovePosition(rb.position + moveDirection * _movementSpeed * Time.deltaTime);
            }
        }

        if (enemy.IsWithinStrikingDistance)
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
    }



    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
