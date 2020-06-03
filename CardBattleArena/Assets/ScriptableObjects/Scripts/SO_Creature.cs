using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature", menuName = "Cards/Creature")]
public class SO_Creature : SO_Card
{
    public CreatureType creatureType;

    public Sprite attackPattern;
    public int movement;
    public int damage;
    public int health;

}
