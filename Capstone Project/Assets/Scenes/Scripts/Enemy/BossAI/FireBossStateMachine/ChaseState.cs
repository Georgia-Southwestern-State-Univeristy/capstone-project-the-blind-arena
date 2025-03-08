using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : FireBossState
{
    public ChaseState(FireBossStateMachine boss) : base(boss) 
    {

    }

    public override void EnterState()
    {
        boss.animator.SetFloat("speed", 1);
    }

    public override void UpdateState()
    {
        Vector3 direction = (boss.target.position - boss.transform.position).normalized;
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, boss.target.position, boss.speed * Time.deltaTime);
        boss.FlipSprite(direction.x);

        if (Vector3.Distance(boss.transform.position, boss.target.position) <= boss.minimumDistance)
        {
            boss.ChangeState(new DashAttackState(boss));
        }
    }

    public override void ExitState() { }
}
