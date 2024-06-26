using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
    [SerializeField] protected int health = 100;
    [SerializeField] protected int mana = 100;
    [SerializeField] protected int level = 1;
    private int damage = 10;
    // private float attackSpeed = 1.0f;
    // private float attackRange = 2.0f;
    // private float sightRange = 50.0f;
    // private float chaseSpeed = 5.0f;
    // private float attackCooldown = 0.0f;
    // private float attackCooldownTime = 1.0f;
    // private bool isAttacking = false;

    protected abstract void TakeDamage(CharacterStats source, int damage);


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
                TakeDamage(other.GetComponent<ProjectileController>().attackerStats, other.GetComponent<ProjectileController>().GetDamage());
                break;

            case "MeleeAttack":
                MeleeController meleeController = other.GetComponent<MeleeController>();
                if(!meleeController.GetHitEnemies().Contains(gameObject))       // Prevents multiple hits from the same attack
                {
                    Debug.Log("Hit by " + other.gameObject.name);
                    meleeController.GetHitEnemies().Add(gameObject);
                    TakeDamage(meleeController.attackerStats, meleeController.GetDamage());
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

    protected abstract void Die();
}
