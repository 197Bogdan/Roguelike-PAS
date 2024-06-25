using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private int health = 100;
    private int damage = 10;
    // private float attackSpeed = 1.0f;
    // private float attackRange = 2.0f;
    // private float sightRange = 50.0f;
    // private float chaseSpeed = 5.0f;
    // private float attackCooldown = 0.0f;
    // private float attackCooldownTime = 1.0f;
    // private bool isAttacking = false;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Took " + damage + " damage! Current health: " + health);
        if(health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Killed enemy!");
        }
    }

    public int GetDamage()
    {
        return damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        

        string tag = other.gameObject.tag;
        switch(tag)
        {
            case "ProjectileAttack":
                TakeDamage(other.GetComponent<ProjectileController>().GetDamage());
                break;

            case "MeleeAttack":
                MeleeController meleeController = other.GetComponent<MeleeController>();
                if(!meleeController.GetHitEnemies().Contains(gameObject))       // Prevents multiple hits from the same attack
                {
                    Debug.Log("Hit by " + other.gameObject.name);
                    meleeController.GetHitEnemies().Add(gameObject);
                    TakeDamage(meleeController.GetDamage());
                }
                else
                {
                    Debug.Log("Already hit by this attack!");
                }
                break;

            default:
                break;
        }
    }
}
