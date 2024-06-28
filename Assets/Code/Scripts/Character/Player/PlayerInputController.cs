using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerController playerController;
    public MenuManager menuManager;
    
    private InputAction movementInput;
    private InputAction dashInput;
    private InputAction meleeAttackAction;
    private InputAction hotbar1Action;
    private InputAction hotbar2Action;
    private InputAction hotbar3Action;
    private InputAction pauseAction;

    private string movementActionName = "Move";
    private string dashActionName = "Dash";
    private string meleeAttackActionName = "MeleeAttack";
    private string hotbar1ActionName = "Hotbar1";
    private string hotbar2ActionName = "Hotbar2";
    private string hotbar3ActionName = "Hotbar3";
    private string pauseActionName = "Pause";


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
        pauseAction = playerInput.actions.FindAction(pauseActionName);
        
        dashInput.performed += ctx => playerController.TriggerDash();
        meleeAttackAction.performed += ctx => playerController.TriggerMeleeAttack();
        hotbar1Action.performed += ctx => playerController.UseHotbar(0);
        hotbar2Action.performed += ctx => playerController.UseHotbar(1);
        hotbar3Action.performed += ctx => playerController.UseHotbar(2);
        pauseAction.performed += ctx => menuManager.PauseGame();

    }

    public Vector2 GetMovementInput()
    {
        return movementInput.ReadValue<Vector2>();
    }

}




