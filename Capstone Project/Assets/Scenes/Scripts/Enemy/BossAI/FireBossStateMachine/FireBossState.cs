using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FireBossState
{
    protected FireBossStateMachine boss;
    protected FireBossStateMachine stateMachine;

    public FireBossState(FireBossStateMachine boss)
    {
        this.boss = boss;
        this.stateMachine = boss;
    }


    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
