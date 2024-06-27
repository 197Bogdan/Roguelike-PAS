using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private int damage = 10;
    public CharacterStats attackerStats;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == attackerStats.gameObject)
        {
            return;
        }

        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
