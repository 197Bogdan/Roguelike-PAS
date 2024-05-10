using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    private IdleState idleState;
    private AttackState attackState;

    void Start()
    {
        idleState = GetComponent<IdleState>();
        attackState = GetComponent<AttackState>();
        animator = GetComponent<Animator>();
        Debug.Log("Base state start");
    }

    public override void RunState()
    {
        Debug.Log("Chasing player");
        if (!CanSeePlayer())
        {
            animator.SetBool("isChasing", false);
            nextState = idleState;
        }
        else if (CanAttackPlayer())
        {
            animator.SetBool("isAttacking", true);
           // animator.SetBool("isChasing", false);
            nextState = attackState;
        }
        else
        {
            nextState = this;
        }
    }

    public override BaseState GetNextState()
    {
        return nextState;
    }
}
