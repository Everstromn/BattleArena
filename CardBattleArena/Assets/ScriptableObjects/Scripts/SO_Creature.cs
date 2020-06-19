using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature", menuName = "Cards/Creature")]
public class SO_Creature : SO_Card
{
    public CreatureType creatureType;

    [Space(20)]
    [Header("Creature Card Attack Info")]
    public int attackRange;
    public int damage;

    [Space(20)]
    [Header("Creature Defence Settings")]
    public int health;
    public bool retaliates = false;
    public int retaliationDamage = 0;
    public bool regenerates = false;
    public int regenerationAmount = 0;

    [Space(20)]
    [Header("Creature Movement Settings")]
    public int movement;
    public bool hasHaste = false;

    [Space(20)]
    [Header("Creature Type Mechanics")]
    public int plantCount;
    public int plantDuration;
    public int plantMovementBuff;
    public int plantRangeBuff;
    public int plantAttackBuff;
    public int plantHealthBuff;
}
