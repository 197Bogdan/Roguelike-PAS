using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatStats : MonoBehaviour
{
    [SerializeField] private int health = 100;
    private int damage = 10;
    private float attackSpeed = 1.0f;
    private float attackRange = 2.0f;
    private float sightRange = 50.0f;
    private float chaseSpeed = 5.0f;
    private float attackCooldown = 0.0f;
    private float attackCooldownTime = 1.0f;
    private bool isAttacking = false;
    private bool isChasing = false;
    private bool isDead = false;
    private bool isStunned = false;
    private bool isBlocking = false;
    private bool isDodging = false;
    private bool isParrying = false;
    private bool isInvincible = false;
    private bool isRanged = false;
    private bool isMelee = false;
    private bool isMagic = false;
    private bool isPhysical = false;
    private bool isFire = false;
    private bool isIce = false;
    private bool isLightning = false;
    private bool isPoison = false;
    private bool isBleeding = false;
    private bool isStaggered = false;
    private bool isKnockedBack = false;
    private bool isKnockedDown = false;
    private bool isGettingUp = false;
    private bool isDeadly = false;
    private bool isVulnerable = false;
    private bool isResistant = false;
    private bool isImmune = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by " + other.name);
        if (other.CompareTag("Attack"))
        {
            Debug.Log("Hit");
        }
    }
}
