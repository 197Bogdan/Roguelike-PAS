using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private int expValue = 10;
    private int manaOnDeath = 30;

    void Start()
    {
        hitbox = GetComponent<Collider>();
    }

    protected override void TakeDamage(CharacterStats attackerStats, int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if(attackerStats is PlayerStats player)
            {
                player.GainExp(expValue);
                player.GainMana(manaOnDeath);
            }

            Die();
        }
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

    public override void UseMana(int amount)
    {
        mana -= amount;
    }

    public override void GainMana(int amount)
    {
        mana += amount;
    }

}
