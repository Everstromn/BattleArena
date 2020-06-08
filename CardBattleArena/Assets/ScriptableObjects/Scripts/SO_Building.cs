using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature", menuName = "Cards/Building")]
public class SO_Building : SO_Card
{
    [Space(20)]
    [Header("Building Card Combat Info")]
    public int attackRange;
    public int damage;
    public int health;

    [Space(20)]
    [Header("Building Economy Settings")]
    public bool impactsGoldPerTurn = false;
    public int goldPerTurnIncrease = 0;

}
