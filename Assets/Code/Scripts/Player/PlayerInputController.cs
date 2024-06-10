using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerController playerController;
    
    private InputAction movementInput;
    private InputAction dashInput;
    private InputAction meleeAttackAction;
    private InputAction hotbar1Action;
    private InputAction hotbar2Action;
    private InputAction hotbar3Action;
    private InputAction hotbar4Action;

    private string movementActionName = "Move";
    private string dashActionName = "Dash";
    private string meleeAttackActionName = "MeleeAttack";
    private string hotbar1ActionName = "Hotbar1";
    private string hotbar2ActionName = "Hotbar2";
    private string hotbar3ActionName = "Hotbar3";
    private string hotbar4ActionName = "Hotbar4";


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();

        movementInput = playerInput.actions.FindAction(movementActionName);
        dashInput = playerInput.actions.FindAction(dashActionName);
        meleeAttackAction = playerInput.actions.FindAction(meleeAttackActionName);
        hotbar1Action = playerInput.actions.FindAction(hotbar1ActionName);
        hotbar2Action = playerInput.actions.FindAction(hotbar2ActionName);
        hotbar3Action = playerInput.actions.FindAction(hotbar3ActionName);
        hotbar4Action = playerInput.actions.FindAction(hotbar4ActionName);
        

        dashInput.performed += ctx => playerController.Dash();
        meleeAttackAction.performed += ctx => playerController.MeleeAttack();
        hotbar1Action.performed += ctx => playerController.UseHotbar(1);
        hotbar2Action.performed += ctx => playerController.UseHotbar(2);
        hotbar3Action.performed += ctx => playerController.UseHotbar(3);
        hotbar4Action.performed += ctx => playerController.UseHotbar(4);

    }

    public Vector2 GetMovementInput()
    {
        return movementInput.ReadValue<Vector2>();
    }

}




