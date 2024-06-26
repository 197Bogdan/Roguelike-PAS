using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats: CharacterStats
{
    public Slider healthBar;
    public Slider manaBar;
    public Slider expBar;

    void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;
        manaBar.maxValue = mana;
        manaBar.value = mana;
        expBar.maxValue = 100;
        expBar.value = exp;
    }

    public override void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player died!");
    }

}