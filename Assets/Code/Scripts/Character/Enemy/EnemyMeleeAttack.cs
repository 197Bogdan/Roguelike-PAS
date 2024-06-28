using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : StateMachineBehaviour
{

    private AttackState attackState;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackState == null)
        {
            attackState = animator.GetComponent<AttackState>();
        }
        attackState.isAttackMoving = true;
        attackState.attackMoveSpeed = Mathf.Min(10f, Vector3.Distance(attackState.transform.position, attackState.player.transform.position) / animator.GetCurrentAnimatorStateInfo(0).length);
        attackState.meleeController.StartAttack();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if(attackState == null)
        {
            attackState = animator.GetComponent<AttackState>();
        }
        attackState.isAttackMoving = false;
        animator.SetBool("isAttacking", false);

        attackState.meleeController.EndAttack();
        attackState.attackMoveSpeed = 0;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
