using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Enemy/Enemy Wave Group")]
public class SO_EnemyWaves : ScriptableObject
{
    public string enemyName;

    public SO_Wave[] enemyWaves;
}
