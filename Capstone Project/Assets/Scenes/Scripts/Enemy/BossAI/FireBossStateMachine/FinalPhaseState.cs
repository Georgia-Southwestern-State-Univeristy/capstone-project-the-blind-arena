using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPhaseState : FireBossState
{
    public FinalPhaseState(FireBossStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        Debug.Log("Entering Final Phase State");

        // Increase the attack speed for the final phase
        stateMachine.baseAttackRate = stateMachine.finalPhaseAttackRate;

        stateMachine.isEnraged = true; // Example mechanic for final phase
    }

    public override void UpdateState()
    {
        if (stateMachine.health <= 0)
        {
            stateMachine.ChangeState(stateMachine.deathState); // Ensure a death state exists
        }
        else
        {
            // Perform final phase actions, like increased attack speed
            stateMachine.PerformFinalPhaseAttacks();
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Final Phase State");

        // Reset attack rate to normal when exiting final phase
        stateMachine.baseAttackRate = 1.5f;
    }
}
