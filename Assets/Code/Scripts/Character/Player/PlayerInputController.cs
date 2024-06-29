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


    void Awake()
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
    }

    void OnEnable()
    {
        dashInput.Enable();
        meleeAttackAction.Enable();
        hotbar1Action.Enable();
        hotbar2Action.Enable();
        hotbar3Action.Enable();
        pauseAction.Enable();

        dashInput.performed += OnDashPerformed;
        meleeAttackAction.performed += OnMeleeAttackPerformed;
        hotbar1Action.performed += ctx => OnHotbarPerformed(ctx, 0);
        hotbar2Action.performed += ctx => OnHotbarPerformed(ctx, 1);
        hotbar3Action.performed += ctx => OnHotbarPerformed(ctx, 2);
        pauseAction.performed += OnPausePerformed;
    }

    void OnDisable()
    {
        dashInput.Disable();
        meleeAttackAction.Disable();
        hotbar1Action.Disable();
        hotbar2Action.Disable();
        hotbar3Action.Disable();
        pauseAction.Disable();

        dashInput.performed -= OnDashPerformed;
        meleeAttackAction.performed -= OnMeleeAttackPerformed;
        hotbar1Action.performed -= ctx => OnHotbarPerformed(ctx, 0);
        hotbar2Action.performed -= ctx => OnHotbarPerformed(ctx, 1);
        hotbar3Action.performed -= ctx => OnHotbarPerformed(ctx, 2);
        pauseAction.performed -= OnPausePerformed;
    }

    public Vector2 GetMovementInput()
    {
        return movementInput.ReadValue<Vector2>();
    }

    public void OnDashPerformed(InputAction.CallbackContext context)
    {
        if(playerController != null)
            playerController.TriggerDash();
    }

    public void OnMeleeAttackPerformed(InputAction.CallbackContext context)
    {
        if(playerController != null)
            playerController.TriggerMeleeAttack();
    }

    public void OnHotbarPerformed(InputAction.CallbackContext context, int slot)
    {
        if(playerController != null)
            playerController.UseHotbar(slot);
    }

    public void OnPausePerformed(InputAction.CallbackContext context)
    {
        if(menuManager != null)
            menuManager.PauseGame();
    }

}




