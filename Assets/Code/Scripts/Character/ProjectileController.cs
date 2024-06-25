using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return damage;
    }
}
