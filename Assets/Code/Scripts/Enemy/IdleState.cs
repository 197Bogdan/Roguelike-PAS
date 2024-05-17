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
        InitValues();
    }
    
    public override void RunState()
    {

        if(!IsInDistance(activeDistance))
        {
            nextState = this;
            return;
        }

        if (IsLineOfSight(sightDistance) || IsInDistance(hearingDistance))
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