using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private int expValue = 10;
    private int manaOnDeath = 30;
    public int type;   // enemy prefab index

    void Start()
    {
        hitbox = GetComponent<Collider>();
    }

    public override void TakeDamage(CharacterStats attackerStats, int damage)
    {
        Instantiate(hitEffectPrefab, transform);
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

    public void SetEnemyType(int type)
    {
        this.type = type;
    }

    public int GetEnemyType()
    {
        return type;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public void SetMana(int mana)
    {
        this.mana = mana;
    }

    public void SetExp(int exp)
    {
        this.exp = exp;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

}
