using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellAttack : StateMachineBehaviour
{

    private AttackState attackState;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackState == null)
        {
            attackState = animator.GetComponent<AttackState>();
        }
        attackState.isAttackMoving = false;
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

        int randomSpell = Random.Range(0, attackState.spells.Length);
        attackState.spells[randomSpell].Cast(attackState.transform, attackState.transform.position + attackState.transform.forward, attackState.GetComponent<CharacterStats>());
        attackState.GetComponent<CharacterStats>().GainMana(attackState.spells[randomSpell].manaCost); // for now, enemies don't use mana

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
