using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttackState : FireBossState
{
    public ProjectileAttackState(FireBossStateMachine boss) : base(boss)
    {
    }

    public override void EnterState()
    {
        boss.StartCoroutine(ProjectileAttackRoutine());
    }

    private IEnumerator ProjectileAttackRoutine()
    {
        boss.isThrowingProjectiles = true;
        while (boss.isThrowingProjectiles)
        {
            if (boss.attackPrefabs.Length > 0)
            {
                GameObject projectile = GameObject.Instantiate(boss.attackPrefabs[0], boss.transform.position, Quaternion.identity);
                Vector3 direction = (boss.target.position - boss.transform.position).normalized;
                boss.FlipSprite(boss.target.position.x);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * boss.projectileSpeed;
                }
            }
            yield return new WaitForSeconds(boss.projectileAttackRate);
        }
    }

    public override void UpdateState() 
    {

    }
    public override void ExitState()
    {
        boss.isThrowingProjectiles = false;
    }
}
