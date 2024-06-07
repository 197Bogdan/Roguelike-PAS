using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float walkingSpeed = 5f;
    [SerializeField] private float dashingSpeed = 4f;

    private PlayerCombatController playerCombatController;
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;

    private Vector2 movementInputVector;
    private Vector3 moveDirection;
    private Vector3 dashDirection;

    private InputAction movementInput;
    private InputAction dashInput;
    private string movementActionName = "Move";
    private string dashActionName = "Dash";

    private bool isDashing = false;

    private LayerMask invisibleFloorLayer;
    private LayerMask playerLayer;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerCombatController = GetComponent<PlayerCombatController>();
        playerInput = GetComponent<PlayerInput>();

        movementInput = playerInput.actions.FindAction(movementActionName);
        dashInput = playerInput.actions.FindAction(dashActionName);

        dashInput.performed += ctx => StartDashing();

        invisibleFloorLayer = LayerMask.GetMask("InvisibleFloor");

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("InvisibleFloor"), true);
    }

    void Update()
    {
        if (playerCombatController.IsAttacking())
            return;

        if (isDashing)
        {
            MovePlayer(dashDirection, dashingSpeed);
            return;
        }

        // Normal movement
        movementInputVector = GetMovementInputVector();
        AnimatePlayerWalk(movementInputVector);
        MovePlayer(movementInputVector, walkingSpeed);
        RotatePlayerTowardsCursor();
    }

    private void MovePlayer(Vector2 inputVector, float speed)
    {
        moveDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;

        characterController.Move(moveDirection.normalized * speed * Time.deltaTime);
    }

    private void RotatePlayerTowardsCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 10f);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, invisibleFloorLayer))
        {
            transform.LookAt(hit.point);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }

    private void AnimatePlayerWalk(Vector2 inputVector)
    {
        // Rotate input vector based on camera rotation to move relative to camera, not world (e.g.: W moves player up the screen, not forward in world space)
        // Rotate opposite to player to keep the input's direction independent of player's rotation (e.g.: player is facing to the right, pressing W should move the player up, not forward(right), so play MoveLeft animation to move up)
        Vector3 rotatedInput = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y - transform.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y);
        rotatedInput.Normalize();

        animator.SetFloat("x", Mathf.Lerp(animator.GetFloat("x"), rotatedInput.x, 0.7f));
        animator.SetFloat("y", Mathf.Lerp(animator.GetFloat("y"), rotatedInput.z, 0.7f)); 
        animator.SetBool("isMoving", inputVector.magnitude > 0);
    }

    private Vector2 GetMovementInputVector()
    {
        return movementInput.ReadValue<Vector2>();
    }

    private void StartDashing()
    {
        if (isDashing || playerCombatController.IsAttacking())
            return;

        dashDirection = GetMovementInputVector();   
        if (dashDirection.magnitude == 0)   // prevent dash if no input
            return;

        isDashing = true;

        animator.SetTrigger("Dash");
        Invoke("OnDashAnimationFinished", 0.15f);
    }

    void OnDashAnimationFinished() 
    {
        isDashing = false;
    }
}
