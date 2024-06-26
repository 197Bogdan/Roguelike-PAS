using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats: CharacterStats
{
    private int currentExp = 0;
    private int expToNextLevel = 100;

    public Slider healthBar;
    public Slider manaBar;
    public Slider expBar;
    public TMPro.TMP_Text levelText;

    void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;
        manaBar.maxValue = mana;
        manaBar.value = mana;
        expBar.maxValue = 100;
        expBar.value = currentExp;
    }

    protected override void TakeDamage(CharacterStats attackerStats, int damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
            Debug.Log("Player died!");
    }

    protected override void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void GainExp(int exp)
    {
        currentExp += exp;
        expBar.value = currentExp;
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;
        currentExp = currentExp - expToNextLevel;
        expToNextLevel = 100 + (level * 50);
        expBar.maxValue = expToNextLevel;
        expBar.value = currentExp;
        levelText.text = "Lvl " + level.ToString();
    }

}