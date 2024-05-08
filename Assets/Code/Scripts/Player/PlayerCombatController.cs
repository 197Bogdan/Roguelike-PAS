using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatController : MonoBehaviour
{
    private Animator animator;
    
    private PlayerInput playerInput;
    private InputAction attackAction;
    private string attackActionName = "MeleeAttack";

    private bool isAttacking = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions.FindAction(attackActionName);
        attackAction.performed += ctx => MeleeAttack();

        animator = GetComponent<Animator>();
    }

    void MeleeAttack()
    {
        if (isAttacking) 
            return;
        
        isAttacking = true;
        animator.SetTrigger("MeleeAttack");
    }

    void OnMeleeAttackAnimationFinished()   // called at ~60% of the animation
    {
        Debug.Log("Melee attack animation finished");
        isAttacking = false;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

}




