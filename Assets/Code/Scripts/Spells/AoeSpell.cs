using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Aoe Spell", menuName = "Spells/Aoe Spell")]
public class AoeSpell: Spell
{
    public int damage;
    public float radius;

    public override void Cast(Transform origin, Vector3 target, CharacterStats attackerStats)
    {
        if(attackerStats.GetMana() < manaCost)
            return;
        
        attackerStats.UseMana(manaCost);

        GameObject spellInstance = Instantiate(spellPrefab, target, Quaternion.identity);

        
    }
}