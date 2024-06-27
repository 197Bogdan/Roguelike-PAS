using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeController: MonoBehaviour
{
    public CharacterStats attackerStats;
    public int damagePerTick;
    public float ticksPerSecond;
    public float radius;
    public int duration;

    private void Start()
    {
        StartCoroutine(DealDamage());
    }

    private IEnumerator DealDamage()
    {
        float tickTime = 1 / ticksPerSecond;
        float endTime = Time.time + duration;

        for(int tickCount = 0; tickCount < duration * ticksPerSecond + 1; tickCount++)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider collider in colliders)
            {
                CharacterStats targetStats = collider.GetComponent<CharacterStats>();
                if (targetStats != null)        // can also detect weapon hitboxes
                {
                    targetStats.TakeDamage(attackerStats, damagePerTick);
                }
            }
            yield return new WaitForSeconds(tickTime);
        }

        Destroy(gameObject);
    }
}