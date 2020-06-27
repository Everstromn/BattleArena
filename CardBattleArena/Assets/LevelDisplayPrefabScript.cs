using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LevelDisplayPrefabScript : MonoBehaviour
{
    [SerializeField] private TMP_Text levelName = null;

    private SO_Level myLevel;


    public void OnSpawn(SO_Level _scene)
    {
        myLevel = _scene;
        levelName.text = myLevel.levelName;
    }

    public void SelectLevel()
    {
        BattleManager.instance.battleLevel = myLevel;
        FindObjectOfType<BattleSelectScreenUI>().UpdateLevelDisplay(myLevel);
        SoundsManager.instance.PlayClickSound();
    }
}
