using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private ChaseState chaseState;

    void Start()
    {
        chaseState = GetComponent<ChaseState>();
    }

    public override void RunState()
    {
        Debug.Log("Attacking player");
        if (!CanAttackPlayer())
        {
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

