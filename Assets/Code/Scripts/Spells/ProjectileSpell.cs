using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Spell", menuName = "Spells/Projectile Spell")]
public class ProjectileSpell: Spell
{
    public int damage;
    public float speed;

    public override void Cast(Transform origin, Vector3 target, CharacterStats attackerStats)
    {
        if(attackerStats.GetMana() < manaCost)
            return;
        
        attackerStats.UseMana(manaCost);

        GameObject spellInstance = Instantiate(spellPrefab, origin.position + new Vector3(0f, 1.5f, 0f), Quaternion.identity);
        spellInstance.transform.LookAt(target);
        spellInstance.transform.rotation = Quaternion.Euler(0, spellInstance.transform.rotation.eulerAngles.y, 0);
        spellInstance.GetComponent<Rigidbody>().velocity = spellInstance.transform.forward * speed;

        ProjectileController projectile = spellInstance.GetComponent<ProjectileController>();
        projectile.attackerStats = attackerStats;
        projectile.SetDamage(damage);

        
    }
}

