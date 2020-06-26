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

    [Space(20)]
    [Header("Unique Building Mechanics")]
    public bool canSpawnToken = false;
    public int tokenSpawnEveryXTturns = 0;
    public SO_Creature tokenToSpawn;
    [Space(10)]
    public bool canChangeNodeType = false;
    public CardType nodeTypeToAdd = CardType.Nuetral;
    [Space(10)]
    public bool actAsUndyingSpawnPoint = false;
}
