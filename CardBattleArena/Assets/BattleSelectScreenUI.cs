using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleSelectScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject deckDisplay = null;
    [SerializeField] private TMP_Text deckDisplayPreview = null;
    [SerializeField] private GameObject deckDisplayPrefab = null;

    [SerializeField] private GameObject levelDisplay = null;
    [SerializeField] private TMP_Text levelDisplayPreview = null;
    [SerializeField] private GameObject levelDisplayPrefab = null;

    [SerializeField] private GameObject enemyDisplay = null;
    [SerializeField] private TMP_Text enemyDisplayPreview = null;
    [SerializeField] private GameObject enemyDisplayPrefab = null;

    public SO_Deck[] availableDecks;
    public SO_Level[] availableLevels;
    public SO_EnemyWaves[] availableEnemies;


    private void Start()
    {
        foreach (SO_Deck deck in availableDecks)
        {
            GameObject newDeck = Instantiate(deckDisplayPrefab, deckDisplay.transform);
            newDeck.GetComponent<DeckDisplayPrefabScript>().OnSpawn(deck);
        }

        foreach (SO_Level scene in availableLevels)
        {
            GameObject newlevel = Instantiate(levelDisplayPrefab, levelDisplay.transform);
            newlevel.GetComponent<LevelDisplayPrefabScript>().OnSpawn(scene);
        }

        foreach (SO_EnemyWaves enemy in availableEnemies)
        {
            GameObject newenemy = Instantiate(enemyDisplayPrefab, enemyDisplay.transform);
            newenemy.GetComponent<EnemyDisplayPrefabScript>().OnSpawn(enemy);
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(BattleManager.instance.battleLevel.sceneIndex);
    }

    public void UpdateDeckDisplay(SO_Deck _deck) { deckDisplayPreview.text = _deck.deckName; }
    public void UpdateLevelDisplay(SO_Level _level) { levelDisplayPreview.text = _level.levelName + " : " + _level.levelDesc; }
    public void UpdateEnemyDisplay(SO_EnemyWaves _enemy) { enemyDisplayPreview.text = _enemy.enemyName; }
}
