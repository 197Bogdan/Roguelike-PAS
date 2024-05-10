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
        Debug.Log("Base state start");
    }
    
    public override void RunState()
    {
        Debug.Log("Idle");
        if (CanSeePlayer())
        {
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