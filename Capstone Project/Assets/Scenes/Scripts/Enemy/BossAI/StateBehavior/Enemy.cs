using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IEnemyMovement, ITriggerCheckable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;

    public float CurrentHealth { get; set; }
    public Rigidbody RB { get; set; }
    public bool IsFacingRight { get; set; } = true;
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    public bool IsAggroed { get; set; }
    public bool IsWithinStrikingDistance { get; set; }

    public Rigidbody ProjectilePrefab;
    public float RandomMovementRange = 5f;
    public float RandomMovementSpeed = 1f;

    private void Awake()
    {
        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
    }


    private void Start()
    {
        CurrentHealth = MaxHealth;

        RB = GetComponent<Rigidbody>();

        RB.constraints = RigidbodyConstraints.FreezePositionY;

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }


    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    public enum AnimationTriggerType
    {
        EnemyDamaged,
        EnemyFootstepSound
    }

    public void AIDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {

    }

    public void MoveEnemy(Vector3 velocity)
    {

        
        RB.linearVelocity = new Vector3(velocity.x, RB.linearVelocity.y, velocity.z);
        CheckForLeftOrRightFacing(velocity);
    }


    public void CheckForLeftOrRightFacing(Vector2 velocity)
    {
        // Only flip when moving in the X direction (ignoring small movements)
        if (Mathf.Abs(velocity.x) > 0.01f)
        {
            if (velocity.x < 0f && IsFacingRight) // Moving left
            {
                // Flip sprite horizontally
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                IsFacingRight = false;
            }
            else if (velocity.x > 0f && !IsFacingRight) // Moving right
            {
                // Flip sprite horizontally
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                IsFacingRight = true;
            }
        }
    }



    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }
}
