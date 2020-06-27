using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyDisplayPrefabScript : MonoBehaviour
{
    [SerializeField] private TMP_Text levelName = null;

    private SO_EnemyWaves myEnemy;


    public void OnSpawn(SO_EnemyWaves _enemy)
    {
        myEnemy = _enemy;
        levelName.text = myEnemy.enemyName;
    }

    public void SelectEnemy()
    {
        BattleManager.instance.enemyDeck = myEnemy;
        BattleManager.instance.UpdateAIWaves();
        FindObjectOfType<BattleSelectScreenUI>().UpdateEnemyDisplay(myEnemy);
        SoundsManager.instance.PlayClickSound();
    }
}
