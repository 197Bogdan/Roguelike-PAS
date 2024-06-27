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
        hitbox = GetComponent<BoxCollider>();

        healthBar.maxValue = health;
        healthBar.value = health;
        manaBar.maxValue = mana;
        manaBar.value = mana;
        expBar.maxValue = 100;
        expBar.value = currentExp;

        StartCoroutine(RepeatingManaLoss());
    }

    protected override void TakeDamage(CharacterStats attackerStats, int damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
            Debug.Log("Player died!");
    }

    public override void UseMana(int amount)
    {
        mana -= amount;
        manaBar.value = mana;
    }

    public override void GainMana(int amount)
    {
        mana += amount;
        manaBar.value = mana;
    }

    private IEnumerator RepeatingManaLoss()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            UseMana(1);
            if(mana <= 0)
            {
                TakeDamage(this, mana * -1);
                mana = 0;
            }
        }
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