using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatController : MonoBehaviour
{
    private Animator animator;
    
    private PlayerInput playerInput;

    private InputAction meleeAttackAction;
    private InputAction hotbar1Action;
    private InputAction hotbar2Action;
    private InputAction hotbar3Action;
    private InputAction hotbar4Action;
    private string meleeAttackActionName = "MeleeAttack";
    private string hotbar1ActionName = "Hotbar1";
    private string hotbar2ActionName = "Hotbar2";
    private string hotbar3ActionName = "Hotbar3";
    private string hotbar4ActionName = "Hotbar4";

    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        GetInputActionReferences();

        meleeAttackAction.performed += ctx => MeleeAttack();
        hotbar1Action.performed += ctx => UseHotbar(1);
        hotbar2Action.performed += ctx => UseHotbar(2);
        hotbar3Action.performed += ctx => UseHotbar(3);
        hotbar4Action.performed += ctx => UseHotbar(4);
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

    void UseHotbar(int slot)
    {
        Debug.Log("Using hotbar slot " + slot);
        Vector3 targetPosition = GetMouseClickPosition();
        SpawnProjectile(targetPosition);
    }

    Vector3 GetMouseClickPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    void SpawnProjectile(Vector3 targetPosition)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = direction.normalized * projectileSpeed;
    }


    public bool IsAttacking()
    {
        return isAttacking;
    }

    void GetInputActionReferences()
    {
        playerInput = GetComponent<PlayerInput>();
        meleeAttackAction = playerInput.actions.FindAction(meleeAttackActionName);
        hotbar1Action = playerInput.actions.FindAction(hotbar1ActionName);
        hotbar2Action = playerInput.actions.FindAction(hotbar2ActionName);
        hotbar3Action = playerInput.actions.FindAction(hotbar3ActionName);
        hotbar4Action = playerInput.actions.FindAction(hotbar4ActionName);
    }
}




