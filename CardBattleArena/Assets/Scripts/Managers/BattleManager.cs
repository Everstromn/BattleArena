using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    private void Awake()
    {

        int gameManagerCount = FindObjectsOfType<BattleManager>().Length;

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
            Debug.LogError("More than one Battle Manager");
        }
        instance = this;
    }

    public GameObject prefabToken;
    public GameObject prefabTokenPreview;
    public GameObject Arena;
    public BattlePhase currentPhase;
    public Team currentTeamTurn;

    public Team playerTeam;
    public Team otherTeam;

    public int remainingActions = 0;
    public int turnCounter = 0;

    private void Start()
    {
        FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();
    }

    public void NextPhase()
    {
        switch(currentPhase)
        {
            case BattlePhase.Upkeep:
                ActivateDrawPhase();
                break;

            case BattlePhase.Draw:
                ActivateOrdersPhase();
                break;

            case BattlePhase.Orders:
                ActivateActionPhase();
                break;

            case BattlePhase.Action:
                ActivateCompletionPhase();
                break;

            case BattlePhase.Completion:
                ActivateUpkeepPhase();
                break;
        }
    }

    private void Update()
    {
        if (remainingActions == 0) { NextPhase(); }
    }

    private void ActivateUpkeepPhase()
    {
        remainingActions++;

        currentPhase = BattlePhase.Upkeep;
        
        CurrencyManager.instance.IncreaseGoldPerTurn(1);
        CurrencyManager.instance.AddTurnGold();
        FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();

        RefreshAllTokens();

        remainingActions = 0;
    }

    private void ActivateDrawPhase()
    {
        remainingActions++;

        currentPhase = BattlePhase.Draw;

        PlayerDeckManager.instance.DrawCardFromDeck();
        FindObjectOfType<BattleArenaUIManager>().UpdateHandDisplay();

        remainingActions = 0;
    }

    private void ActivateOrdersPhase()
    {
        remainingActions++;

        currentPhase = BattlePhase.Orders;
    }

    private void ActivateActionPhase()
    {
        remainingActions++;

        currentPhase = BattlePhase.Action;

        TokenManager[] activeTokens = FindObjectsOfType<TokenManager>();

        foreach (TokenManager token in activeTokens)
        {
            Debug.Log("Looping through Tokens " + token.name + token.myTeam);
            Debug.Log("Current Team Color : " + currentTeamTurn);

            if (token.myTeam == currentTeamTurn)
            {
                remainingActions++;
                token.ActionPhase();
            }
        }

        remainingActions--;
    }

    private void ActivateCompletionPhase()
    {
        remainingActions++;

        currentPhase = BattlePhase.Completion;

        turnCounter++;
        remainingActions = 0;
    }

    public void SpawnTokenFromCard(SO_Card givenCard)
    {
        GameObject newToken = Instantiate(prefabTokenPreview, Arena.transform);
        newToken.GetComponent<PreviewTokenManager>().OnSpawn(givenCard);
    }

    private void RefreshAllTokens()
    {
        TokenManager[] currentTokens = FindObjectsOfType<TokenManager>();
        foreach (TokenManager token in currentTokens)
        {
            token.UpkeepPhase();
        }
    }
}
