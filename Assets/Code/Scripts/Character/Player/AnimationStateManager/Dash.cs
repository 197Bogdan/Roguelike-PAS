using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : StateMachineBehaviour
{
    private PlayerController playerController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playerController == null)
        {
            playerController = animator.GetComponent<PlayerController>();
        }

        playerController.isBufferedDash = false;
        animator.SetBool("isBufferedDash", false);

        if(Time.time < playerController.nextDashTime)
        {
            animator.SetTrigger("cancelDash");
            return;
        }
            
        playerController.isDashing = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(playerController == null)
        {
            playerController = animator.GetComponent<PlayerController>();
        }

        if(playerController.isDashing)  // this state is also reached by trying to dash while on cooldown
        {
            playerController.isDashing = false;
            animator.SetBool("isDashing", false);

            playerController.nextDashTime = Time.time + playerController.dashCooldown;
        }

    }
}
