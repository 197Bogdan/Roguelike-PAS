using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    private ChaseState chaseState;
    private AttackState attackState;

    void Start()
    {
        chaseState = GetComponent<ChaseState>();
        animator = GetComponent<Animator>();
    }
    
    public override void RunState()
    {
        if (CanSeePlayer())
        {
            Debug.Log("Found player");
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