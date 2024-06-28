using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    private ChaseState chaseState;
    public bool isAttackMoving = false;
    public float attackMoveSpeed = 0;
    public MeleeController meleeController;

    void Start()
    {
        chaseState = GetComponent<ChaseState>();
        InitValues();
    }

    public override void RunState()
    {
        if(isAttackMoving == false)
            transform.LookAt(player.transform); // rotate during windup
        else
            transform.position = transform.position + transform.forward * attackMoveSpeed * Time.deltaTime; // move during attack
            


        if(animator.GetBool("isAttacking") == true)     // prevent leaving state before attack action is finished
        {
            nextState = this;
            return;
        }

        nextState = chaseState;
    }

    public override BaseState GetNextState()
    {
        return nextState;
    }
}

