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

        // If aggroed, move towards the player without rotation
        if (enemy.IsAggroed)
        {
            Vector2 moveDirection = (_playerTransform.position - enemy.transform.position).normalized;

            // Only move on the X-axis and ignore Y-axis velocity
            enemy.MoveEnemy(moveDirection * _movementSpeed);
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
