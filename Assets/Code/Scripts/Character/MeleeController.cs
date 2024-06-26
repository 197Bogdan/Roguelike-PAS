using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    private int damage = 10;
    public Collider weaponCollider;
    public CharacterStats attackerStats;
    private List<GameObject> hitEnemies;

    // Start is called before the first frame update
    void Start()
    {
        hitEnemies = new List<GameObject>();
        weaponCollider.enabled = false;
        attackerStats = GetComponentInParent<CharacterStats>();
    }

    public void StartAttack()
    {
        weaponCollider.enabled = true;
        hitEnemies.Clear();
    }

    public void EndAttack()
    {
        weaponCollider.enabled = false;
        hitEnemies.Clear();
    }

    public List<GameObject> GetHitEnemies()
    {
        return hitEnemies;
    }

    public int GetDamage()
    {
        return damage;
    }


}
