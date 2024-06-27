using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Aoe Spell", menuName = "Spells/Aoe Spell")]
public class AoeSpell: Spell
{
    public int damagePerTick;
    public float ticksPerSecond;
    public float radius;
    public int duration;

    public override void Cast(Transform origin, Vector3 target, CharacterStats attackerStats)
    {
        if(attackerStats.GetMana() < manaCost)
            return;
        
        attackerStats.UseMana(manaCost);

        target.y = 0.1f;
        GameObject spellInstance = Instantiate(spellPrefab, target, Quaternion.identity);
        Destroy(spellInstance, duration);
    }
}