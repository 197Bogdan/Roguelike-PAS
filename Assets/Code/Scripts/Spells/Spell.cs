using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell: ScriptableObject
{
    public string spellName;
    public int manaCost;
    // public float castTime;
    public GameObject spellPrefab;

    public abstract void Cast(Transform origin, Vector3 target, CharacterStats attackerStats);
}