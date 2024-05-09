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
    }

    public override void RunState()
    {
        Debug.Log("Chasing player");
        if (!CanSeePlayer())
        {
            nextState = idleState;
        }
        else if (CanAttackPlayer())
        {
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
