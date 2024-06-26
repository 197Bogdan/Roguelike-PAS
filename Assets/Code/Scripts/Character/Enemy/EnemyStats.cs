using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private int expValue = 10;

    protected override void TakeDamage(CharacterStats attackerStats, int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if(attackerStats is PlayerStats player)
                player.GainExp(expValue);
            Die();
        }
    }

    protected override void Die()
    {
        Destroy(gameObject);
    }

}
