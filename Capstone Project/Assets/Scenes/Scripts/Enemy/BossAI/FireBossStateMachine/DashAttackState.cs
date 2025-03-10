using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAttackState : FireBossState
{
    public DashAttackState(FireBossStateMachine boss) : base(boss) { }

    public override void EnterState()
    {
        boss.isDashing = true;
        boss.StartCoroutine(DashAttackRoutine());
    }

    private IEnumerator DashAttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 dashDirection = (boss.target.position - boss.transform.position).normalized;
        float elapsedTime = 0f;

        while (elapsedTime < boss.dashDuration)
        {
            boss.transform.position += dashDirection * boss.dashSpeed * Time.deltaTime;
            boss.FlipSprite(dashDirection.x);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        boss.isDashing = false;
        boss.ChangeState(new ChaseState(boss));
    }

    public override void UpdateState() { }
    public override void ExitState() { }
}
