using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : BaseState
{
    private IdleState idleState;
    private AttackState attackState;

    void Start()
    {
        idleState = GetComponent<IdleState>();
        attackState = GetComponent<AttackState>();
        InitValues();
    }

    public override void RunState()
    {

        if (IsLineOfSight(attackDistance))
        {
            animator.SetBool("isAttacking", true);
            nextState = attackState;
            // agent.SetDestination(transform.position);
            agent.isStopped = true;
        }
        else
        {
            if(agent.isStopped)
                agent.isStopped = false;
            agent.SetDestination(player.transform.position);
            nextState = this;
        }
    }

    public override BaseState GetNextState()
    {
        return nextState;
    }
}
