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
        attackState = GetComponent<AttackState>();
    }
    
    public override void RunState()
    {
        Debug.Log("Idle");
        if (CanSeePlayer())
        {
            if (CanAttackPlayer())
            {
                nextState = attackState;
            }
            else
            {
                nextState = chaseState;
            }
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