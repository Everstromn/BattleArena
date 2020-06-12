using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Enemy/Wave")]
public class SO_Wave : ScriptableObject
{
    public int spawnWave;
    public SO_Creature[] spawnCreatures;
}
