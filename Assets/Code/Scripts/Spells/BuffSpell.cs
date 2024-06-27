using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Spell", menuName = "Spells/Buff Spell")]
public class BuffSpell: Spell
{
    public int duration;
    public int buffAmount;
    public BuffType buffType;

    public override void Cast(Transform origin, Vector3 target, CharacterStats attackerStats)
    {
        if(attackerStats.GetMana() < manaCost)
            return;
        
        attackerStats.UseMana(manaCost);
        attackerStats.AddBuff(buffType, buffAmount, duration);
    }
}