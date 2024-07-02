using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    public int damage = 10;
    public Collider weaponCollider;
    public CharacterStats attackerStats;
    private List<GameObject> hitEnemies;
    public GameObject slashEffectPrefab;
    private GameObject slashEffect;

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
        if(slashEffectPrefab != null)
            slashEffect = Instantiate(slashEffectPrefab, attackerStats.transform.position + new Vector3(0, 1, 0) + attackerStats.transform.forward, attackerStats.transform.rotation * Quaternion.Euler(0, 0, 180));
    }

    public void EndAttack()
    {
        weaponCollider.enabled = false;
        hitEnemies.Clear();
        if(slashEffectPrefab != null)
        Destroy(slashEffect);
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
