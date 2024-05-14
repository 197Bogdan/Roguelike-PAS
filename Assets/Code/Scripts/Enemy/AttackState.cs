using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private ChaseState chaseState;

    void Start()
    {
        chaseState = GetComponent<ChaseState>();
        animator = GetComponent<Animator>();
    }

    public override void RunState()
    {
        if (!CanAttackPlayer())
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isChasing", true);
            nextState = chaseState;
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

