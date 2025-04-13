using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : FireBossState
{
    public IdleState(FireBossStateMachine boss) : base(boss) 
    { 

    }

    public override void EnterState()
    {
        boss.animator.SetFloat("speed", 0);
    }

    public override void UpdateState()
    {
        if (Vector3.Distance(boss.transform.position, boss.target.position) <= boss.minimumDistance)
        {
            boss.ChangeState(new DashAttackState(boss));
        }
        else
        {
            boss.ChangeState(new ChaseState(boss));
        }
    }

    public override void ExitState() { }
}
