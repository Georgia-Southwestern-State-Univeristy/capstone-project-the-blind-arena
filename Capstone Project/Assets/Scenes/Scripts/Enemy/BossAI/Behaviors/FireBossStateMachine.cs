using System;
using UnityEngine;

public enum FireBossState
{
    Idle,
    Attack,
    Defend,
    Chase,
    Retreat
}

public class FireBossStateMachine
{
    private FireBossState currentState;
    private UnityEngine.Vector3 playerPosition;
    private UnityEngine.Vector3 fireBossPosition; // Assuming this variable is defined elsewhere
    private float attackRange; // Assuming this variable is defined elsewhere
    private float chaseRange; // Assuming this variable is defined elsewhere

    public FireBossStateMachine()
    {
        currentState = FireBossState.Idle;
    }

    public void Update(UnityEngine.Vector3 playerPos)
    {
        playerPosition = playerPos;
        switch (currentState)
        {
            case FireBossState.Idle:
                HandleIdle();
                break;
            case FireBossState.Attack:
                HandleAttack();
                break;
            case FireBossState.Defend:
                HandleDefend();
                break;
            case FireBossState.Chase:
                HandleChase();
                break;
            case FireBossState.Retreat:
                HandleRetreat();
                break;
        }

        // Decision logic based on player position
        float distanceToPlayer = UnityEngine.Vector3.Distance(playerPosition, fireBossPosition);
        if (distanceToPlayer < attackRange)
        {
            ChangeState(FireBossState.Attack);
        }
        else if (distanceToPlayer < chaseRange)
        {
            ChangeState(FireBossState.Chase);
        }
        else
        {
            ChangeState(FireBossState.Idle);
        }
    }

    public void ChangeState(FireBossState newState)
    {
        currentState = newState;
    }

    private void HandleIdle()
    {
        // Logic for Idle state
        Console.WriteLine("Fire Boss is idling.");
    }

    private void HandleAttack()
    {
        // Logic for Attack state
        Console.WriteLine("Fire Boss is attacking!");
    }

    private void HandleDefend()
    {
        // Logic for Defend state
        Console.WriteLine("Fire Boss is defending!");
    }

    private void HandleChase()
    {
        // Logic for Chase state
        Console.WriteLine("Fire Boss is chasing the player!");
    }

    private void HandleRetreat()
    {
        // Logic for Retreat state
        Console.WriteLine("Fire Boss is retreating!");
    }
}
