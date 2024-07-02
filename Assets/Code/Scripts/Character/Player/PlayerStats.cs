using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats: CharacterStats
{
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
        expBar.value = exp;

        StartCoroutine(RepeatingManaLoss());
    }

    public override void TakeDamage(CharacterStats attackerStats, int damage)
    {
        Instantiate(hitEffectPrefab, transform);
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
            Die();
    }

    public override void UseMana(int amount)
    {
        mana -= amount;
        manaBar.value = mana;
    }

    public override void GainMana(int amount)
    {
        mana += amount;
        if(mana > maxMana)
        {
            TakeDamage(this, maxMana - mana);   // heal excess mana as health
            mana = maxMana;
        }

        manaBar.value = mana;
    }

    public void GainHealth(int amount)
    {
        health += amount;
        if(health > maxHealth)
        {
            health = maxHealth;
        }

        healthBar.value = health;
    }

    private IEnumerator RepeatingHealthRegen()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            GainHealth(10);
        }
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
        SaveManager saveManager = FindObjectOfType<SaveManager>();
        saveManager.DeleteSave();
    }

    public void GainExp(int exp)
    {
        this.exp += exp;
        expBar.value = this.exp;
        if (this.exp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level++;
        exp = exp - expToNextLevel;
        expToNextLevel = 100 + (level * 50);
        expBar.maxValue = expToNextLevel;
        expBar.value = exp;
        levelText.text = "Lvl " + level.ToString();
    }

    public void SetLevel(int level)
    {
        this.level = level;
        levelText.text = "Lvl " + level.ToString();
    }

    public void SetExp(int exp)
    {
        this.exp = exp;
        expBar.value = exp;
    }

    public void SetHealth(int health)
    {
        this.health = health;
        healthBar.value = health;
    }

    public void SetMana(int mana)
    {
        this.mana = mana;
        manaBar.value = mana;
    }



}