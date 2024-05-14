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
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public override void RunState()
    {
        if (!CanSeePlayer())
        {
            Debug.Log("Lost player");
            animator.SetBool("isChasing", false);
            nextState = idleState;
            agent.SetDestination(transform.position);
        }
        else if (CanAttackPlayer())
        {
            animator.SetBool("isAttacking", true);
           // animator.SetBool("isChasing", false);
            nextState = attackState;
            agent.SetDestination(transform.position);
        }
        else
        {
            agent.SetDestination(player.transform.position);
            nextState = this;
        }
    }

    public override BaseState GetNextState()
    {
        return nextState;
    }
}
