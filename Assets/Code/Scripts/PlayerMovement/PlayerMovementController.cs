using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float speed = 0.25f;

    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;

    private Vector2 inputVector;
    private Vector3 moveDirection;

    private InputAction movementInput;
    private string movementActionName = "Move";

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        movementInput = playerInput.actions.FindAction(movementActionName);
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        inputVector = GetInputVector();
        AnimatePlayer(inputVector);
        MovePlayer(inputVector);
        RotatePlayerTowardsCursor();
    }

    private void MovePlayer(Vector2 inputVector)
    {
        moveDirection = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;

        characterController.Move(moveDirection.normalized * speed * Time.fixedDeltaTime);
    }

    private void RotatePlayerTowardsCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.LookAt(hit.point);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }

    private void AnimatePlayer(Vector2 inputVector)
    {
        // Rotate input vector based on camera rotation to move relative to camera, not world (e.g.: W moves player up the screen, not forward in world space)
        // Rotate opposite to player to keep the input's direction independent of player's rotation (e.g.: player is facing to the right, pressing W should move the player up, not forward(right), so play MoveLeft animation to move up)
        Vector3 rotatedInput = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y - transform.eulerAngles.y, 0) * new Vector3(inputVector.x, 0, inputVector.y);
        rotatedInput.Normalize();

        animator.SetFloat("x", rotatedInput.x);
        animator.SetFloat("y", rotatedInput.z); 
        animator.SetBool("isMoving", inputVector.magnitude > 0);
    }

    private Vector2 GetInputVector()
    {
        return movementInput.ReadValue<Vector2>();
    }
}
