using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class BattleEndUI : MonoBehaviour
{
    public int mainMenuSceneIndex = 0;

    [SerializeField] private TMP_Text titleTextObj = null;
    [SerializeField] private TMP_Text turnNoTextObj = null;
    [SerializeField] private TMP_Text playerHealthTextObj = null;

    public void UpdateDisplay(string result, int turnNo, int playerHealth)
    {
        titleTextObj.text = result;
        turnNoTextObj.text = turnNo.ToString();
        playerHealthTextObj.text = playerHealth.ToString();
    }

    public void LoadMainMenu()
    {
        BattleManager.instance.battleActive = false;
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

}
