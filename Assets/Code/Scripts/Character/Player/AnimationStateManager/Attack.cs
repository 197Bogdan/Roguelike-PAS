using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : StateMachineBehaviour
{
    
    private PlayerController playerController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playerController == null)
        {
            playerController = animator.GetComponent<PlayerController>();
        }

        playerController.RotatePlayerTowardsCursor();
        playerController.meleeController.StartAttack();

        animator.SetBool("isBufferedAttack", false);
        playerController.isBufferedAttack = false;
        playerController.isAttacking = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playerController == null)
        {
            playerController = animator.GetComponent<PlayerController>();
        }

        playerController.meleeController.EndAttack();
        

        if(!playerController.isBufferedAttack)
        {
            playerController.isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
    }
}
