using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 15f;
    [SerializeField] private float dashingSpeed = 60f;
    [SerializeField] private float attackingMoveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 25f; 

    [SerializeField] private List<Spell> hotbarSpells;

    private PlayerInputController playerInput;
    private PlayerStats playerStats;
    public MeleeController meleeController;
    private CharacterController characterController;
    private Animator animator;

    public MenuManager menuManager;

    private Vector2 movementInput;
    private Vector3 movementDirection;
    private Vector3 dashDirection;

    private LayerMask invisibleFloorLayer;

    public bool isDashing = false;
    public float nextDashTime = 0f;
    public float dashCooldown = 1f;
    public bool isBufferedDash = false;
    public bool isAttacking = false;
    public bool isBufferedAttack = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInputController>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        meleeController = GetComponentInChildren<MeleeController>();
        playerStats = GetComponent<PlayerStats>();

        invisibleFloorLayer = LayerMask.GetMask("InvisibleFloor");
    }

    void Update()
    {
        if(menuManager.IsPaused())
            return;

        if(transform.position.y != 0)
            characterController.Move(new Vector3(0, -transform.position.y, 0));

        if (isAttacking)
        {
            characterController.Move(attackingMoveSpeed * transform.forward * Time.deltaTime);
            return;
        }

        RotatePlayerTowardsCursorLerp();

        if (isDashing)
        {
            MovePlayer(dashDirection, dashingSpeed);
            return;
        }

        // Normal movement
        movementInput = playerInput.GetMovementInput();
        AnimatePlayerWalk(movementInput);
        MovePlayer(movementInput, walkingSpeed);
        
    }

    private void MovePlayer(Vector2 inputVector, float speed)
    {
        movementDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        movementDirection = Camera.main.transform.TransformDirection(movementDirection);
        movementDirection.y = 0;

        float speedModifier = 1f;
        float angle = Vector3.Angle(transform.forward, movementDirection);
        if(angle > 30)
            speedModifier = 0.85f;
        if(angle > 60)
            speedModifier = 0.7f;
        
        characterController.Move(movementDirection.normalized * speed * speedModifier * Time.deltaTime);
    }

    public void RotatePlayerTowardsCursorLerp()
    {
        Vector3 targetPosition = GetCursorPosition();
        if (targetPosition == Vector3.zero)
            return;

        Vector3 directionToTarget = targetPosition - transform.position;
        directionToTarget.y = 0f; 

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void RotatePlayerTowardsCursor()
    {
        Vector3 targetPosition = GetCursorPosition();
        if (targetPosition == Vector3.zero)
            return;

        transform.LookAt(targetPosition);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    Vector3 GetCursorPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, invisibleFloorLayer))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private void AnimatePlayerWalk(Vector2 inputVector)
    {
        // Rotate input vector based on camera rotation to move relative to camera, not world (e.g.: W moves player up the screen, not forward in world space)
        // Rotate opposite to player to keep the input's direction independent of player's rotation (e.g.: player is facing to the right, pressing W should move the player up, not forward(right), so play MoveLeft animation to move up)
        Vector3 rotatedInput = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y - transform.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y);
        rotatedInput.Normalize();

        animator.SetFloat("x", Mathf.Lerp(animator.GetFloat("x"), rotatedInput.x, 0.05f));
        animator.SetFloat("y", Mathf.Lerp(animator.GetFloat("y"), rotatedInput.z, 0.05f)); 
        animator.SetBool("isMoving", inputVector.magnitude > 0);
    }

    public void TriggerDash()
    {
        if(menuManager.IsPaused())
            return;

        if (isDashing)
            return;

        dashDirection = playerInput.GetMovementInput(); 
        if (dashDirection.magnitude == 0)   // prevent dash if no input
            return;

        if(isAttacking)
        {
            isBufferedDash = true;
            animator.SetBool("isBufferedDash", true);
            return;
        }

        if(Time.time < nextDashTime)
            return;

        animator.SetBool("isDashing", true);

    }

    public void TriggerMeleeAttack()
    {
        if(menuManager.IsPaused())
            return;

        if(isDashing || isAttacking)
        {
            isBufferedAttack = true;
            animator.SetBool("isBufferedAttack", true);
            return;
        }

        animator.SetBool("isAttacking", true);
    }

    public void UseHotbar(int slot)
    {
        if(menuManager.IsPaused())
            return;

        Vector3 targetPosition = GetCursorPosition();
        hotbarSpells[slot].Cast(transform, targetPosition, playerStats);
    }

}
