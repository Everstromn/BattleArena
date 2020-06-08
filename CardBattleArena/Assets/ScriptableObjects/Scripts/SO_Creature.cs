using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature", menuName = "Cards/Creature")]
public class SO_Creature : SO_Card
{
    [Space(20)]
    [Header("Creature Card Combat Info")]

    public CreatureType creatureType;

    public int attackRange;
    public int movement;
    public int damage;
    public int health;

}
