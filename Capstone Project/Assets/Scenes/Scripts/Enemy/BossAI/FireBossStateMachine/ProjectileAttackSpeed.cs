using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttackSpeed : FireBossState
{
    public ProjectileAttackSpeed(FireBossStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        Debug.Log("Boss Preparing Projectile Attack");
        stateMachine.isThrowingProjectiles = true;
        stateMachine.StartProjectileAttack();
    }

    public override void UpdateState()
    {
        if (!stateMachine.isThrowingProjectiles)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Boss Finished Throwing Projectiles");
        stateMachine.isThrowingProjectiles = false;
    }
}

