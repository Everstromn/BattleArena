using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;
    private void Awake()
    {

        int gameManagerCount = FindObjectsOfType<CurrencyManager>().Length;

        if (gameManagerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        if (instance != null)
        {
            Debug.LogError("More than one Currency Manager");
        }
        instance = this;
    }

    private int gold = 4;
    public int goldPerTurnBase = 2;
    public int goldPerTurnAddition = 0;

    public void AlterGold(int val) { gold = gold + val; }
    public int ReturnGold() { return gold; }

    public void AddTurnGold()
    {
        int turnGold = goldPerTurnBase + Mathf.Clamp(goldPerTurnAddition, 0, 999);
        gold = gold + turnGold;
    }

    public void IncreaseGoldPerTurn(int val)
    {
        goldPerTurnAddition = goldPerTurnAddition + val;
    }

    public void ResetCurrencyState()
    {
        goldPerTurnAddition = 0;
        gold = 4;
    }

}
