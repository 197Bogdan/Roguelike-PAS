using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
    [SerializeField] public int health = 100;
    [SerializeField] public int mana = 100;
    [SerializeField] protected int level = 1;
    public int damage = 10;
    public Collider hitbox;
    // private float attackSpeed = 1.0f;
    // private float attackRange = 2.0f;
    // private float sightRange = 50.0f;
    // private float chaseSpeed = 5.0f;
    // private float attackCooldown = 0.0f;
    // private float attackCooldownTime = 1.0f;
    // private bool isAttacking = false;

    public abstract void TakeDamage(CharacterStats source, int damage);


    public int GetDamage()
    {
        return damage;
    }

    public int GetMana()
    {
        return mana;
    }

    public abstract void UseMana(int amount);

    public abstract void GainMana(int amount);

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        switch(tag)
        {
            case "ProjectileAttack":
                if(other.GetComponent<ProjectileController>().attackerStats.gameObject != gameObject)   // prevent self hit
                    TakeDamage(other.GetComponent<ProjectileController>().attackerStats, other.GetComponent<ProjectileController>().GetDamage());
                break;

            case "MeleeAttack":
                MeleeController meleeController = other.GetComponent<MeleeController>();
                if(!meleeController.GetHitEnemies().Contains(gameObject))       // Prevents multiple hits from the same attack
                {
                    meleeController.GetHitEnemies().Add(gameObject);
                    TakeDamage(meleeController.attackerStats, meleeController.attackerStats.GetDamage());
                }
                break;

            default:
                break;
        }
    }

    protected abstract void Die();

    public void AddBuff(BuffType buffType, int amount, float duration)
    {
        StartCoroutine(RemoveBuff(buffType, amount, duration));
        switch(buffType)
        {
            case BuffType.Damage:
                damage += amount;
                break;

            case BuffType.Health:
                health += amount;
                break;

            default:
                break;
        }
    }

    private IEnumerator RemoveBuff(BuffType buffType, int amount, float duration)
    {
        yield return new WaitForSeconds(duration);
        switch(buffType)
        {
            case BuffType.Damage:
                damage -= amount;
                break;

            case BuffType.Health:
                health -= amount;
                break;

            default:
                break;
        }
    }

}

public enum BuffType
{
    Damage,
    Health,
}
