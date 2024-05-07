using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float speed = 0.25f;

    private PlayerInput playerInput;
    private InputAction movementInput;
    private string movementActionName = "Move";

    private Vector2 inputVector;
    private Vector3 moveDirection;

    private CharacterController characterController;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        movementInput = playerInput.actions.FindAction(movementActionName);
        characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        RotatePlayer();
        MovePlayer();
    }

    private void MovePlayer()
    {
        inputVector = movementInput.ReadValue<Vector2>();
        Vector3 direction = new Vector3(inputVector.x, 0, inputVector.y).normalized;
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = 0;

        characterController.Move(direction.normalized * speed * Time.fixedDeltaTime);
    }

    private void RotatePlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.LookAt(hit.point);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}
