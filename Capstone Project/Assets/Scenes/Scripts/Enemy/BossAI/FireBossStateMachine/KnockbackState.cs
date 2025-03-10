using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackState : FireBossState
{
    public KnockbackState(FireBossStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        Debug.Log("Boss Knocked Back");
        stateMachine.ApplyKnockbackForce();
    }

    public override void UpdateState()
    {
        if (stateMachine.RecoveredFromKnockback)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Recovered from Knockback");
    }

    public void ApplyKnockbackForce()
    {
        // Add logic for applying knockback here
    }

}
