using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Cards/Spell")]
public class SO_Spell : SO_Card
{
    public SpellType spellType;

    [Space(20)]
    [Header("Spell Affect Range")]
    public int xMin;
    public int xMax;
    public int yMin;
    public int yMax;

    [Space(20)]
    [Header("Damage Spell")]
    public int damageAmount;

    [Space(20)]
    [Header("Spawn Spell")]
    public SO_Card spawnToken;

    [Space(20)]
    [Header("Buff Spell")]
    public int buffLength;
    public int rangeBuff;
    public int movementBuff;
    public int attackBuff;
    public int healthBuff;

    [Space(20)]
    [Header("Resource Spell")]
    public int goldPerAllyCreature;
}
