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

    public int playerStartingHealth;

    public int remainingActions = 0;
    public int turnCounter = 0;

    public float minPhaseTime = 0.5f;

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
        StartCoroutine(MinDelayPhase());
        currentPhase = BattlePhase.Upkeep;

        turnCounter++;
        CurrencyManager.instance.IncreaseGoldPerTurn(1);
        CurrencyManager.instance.AddTurnGold();
        FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();

        RefreshAllTokens();

        remainingActions--;
    }

    private void ActivateDrawPhase()
    {
        remainingActions++;
        StartCoroutine(MinDelayPhase());
        currentPhase = BattlePhase.Draw;

        PlayerDeckManager.instance.DrawCardFromDeck();
        FindObjectOfType<BattleArenaUIManager>().UpdateHandDisplay();

        remainingActions--;
    }

    private void ActivateOrdersPhase()
    {
        remainingActions++;

        currentPhase = BattlePhase.Orders;
    }

    private void ActivateActionPhase()
    {
        remainingActions++;
        StartCoroutine(MinDelayPhase());
        currentPhase = BattlePhase.Action;

        TokenManager[] activeTokens = FindObjectsOfType<TokenManager>();

        foreach (TokenManager token in activeTokens)
        {
            float currentDelay = 0;

            if (token.myTeam == currentTeamTurn)
            {
                remainingActions++;
                currentDelay = token.ActionPhase(currentDelay);
            }
        }

        remainingActions--;
    }

    private void ActivateCompletionPhase()
    {
        remainingActions++;
        StartCoroutine(MinDelayPhase());
        currentPhase = BattlePhase.Completion;

        remainingActions--;
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

    private IEnumerator MinDelayPhase()
    {
        remainingActions++;
        yield return new WaitForSeconds(minPhaseTime);
        remainingActions--;
    }
}
