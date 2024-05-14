using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private int damage = 10;

    public LayerMask excludedLayer;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider's gameObject is not in the excluded layer
        if (((1 << other.gameObject.layer) & excludedLayer) == 0)
        {
            Debug.Log(other.gameObject.layer + " " + excludedLayer.value);
            // Trigger behavior for colliders not in the excluded layer
            Debug.Log("Hit " + other.gameObject.name);
            Destroy(gameObject);
        }
    }

    public int GetDamage()
    {
        return damage;
    }
}
