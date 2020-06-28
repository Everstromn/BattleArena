using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    public bool battleActive;
    [SerializeField] private float battleStartDelay = 5;

    public GameObject prefabCreatureToken;
    public GameObject prefabBuildingToken;

    public SO_Level battleLevel;

    public GameObject prefabCreatureTokenPreview;
    public GameObject prefabBuildingTokenPreview;
    public GameObject prefabSpellTokenPreview;

    private GameObject newToken;

    public GameObject Arena;
    public BattlePhase currentPhase;
    public Team currentTeamTurn;

    public Team playerTeam;
    public Color playerTeamColor = Color.blue;
    public Team otherTeam;
    public Color otherTeamColor = Color.red;

    public Node playerHQNode;
    public Node AIHQNode;

    public int playerStartingHealth;
    public int cardsPerDraw = 2;
    public int maxHandSize = 5;
    public int goldIncomeIncrementEveryXTurns = 3;

    public int remainingActions = 0;
    public int turnCounter = 0;

    public SO_EnemyWaves enemyDeck;

    private int waveOneSpawn = 3;
    private List<SO_Card> waveOneSpawnTokens = new List<SO_Card>();

    private int waveTwoSpawn = 3;
    private List<SO_Card> waveTwoSpawnTokens = new List<SO_Card>();

    private int waveThreeSpawn = 3;
    private List<SO_Card> waveThreeSpawnTokens = new List<SO_Card>();

    private int recurringWaveSpawn = 3;
    private List<SO_Card> recurringWaveSpawnTokens = new List<SO_Card>();

    private int randomNodeIndex; //used for ai random token spawn selection

    public float minPhaseTime = 0.5f;

    private void Start()
    {
        battleActive = false;
    }

    public IEnumerator BattleInitiationDelay()
    {
        yield return new WaitForSeconds(battleStartDelay);
        battleActive = true;
        FindObjectOfType<BattleArenaUIManager>().RotateCameraPos(0);
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
        if (remainingActions == 0 && battleActive) { NextPhase(); }
    }

    private void ActivateUpkeepPhase()
    {
        remainingActions++;
        StartCoroutine(MinDelayPhase());
        currentPhase = BattlePhase.Upkeep;

        if (currentTeamTurn == playerTeam) { currentTeamTurn = otherTeam; } else { currentTeamTurn = playerTeam; }

        RefreshAllTokens();

        if (currentTeamTurn == playerTeam)
        {
            turnCounter++;
            CurrencyManager.instance.AddTurnGold();
            // increase gold income by 1 every 3rd turn
            if (turnCounter % goldIncomeIncrementEveryXTurns == 0)
            {
                CurrencyManager.instance.IncreaseGoldPerTurn(1);
            }
            FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();
        }
        
        remainingActions--;
    }

    private void ActivateDrawPhase()
    {
        remainingActions++;
        StartCoroutine(MinDelayPhase());
        currentPhase = BattlePhase.Draw;

        if (currentTeamTurn == playerTeam)
        {
            if(turnCounter == 1) { PlayerDeckManager.instance.DrawCardFromDeck(); PlayerDeckManager.instance.DrawCardFromDeck(); }

            for (int i = 0; i < cardsPerDraw; i++)
            {
                if (PlayerDeckManager.instance.playerHand.Count < maxHandSize)
                {

                    PlayerDeckManager.instance.DrawCardFromDeck();
                }
            }

        }

        FindObjectOfType<BattleArenaUIManager>().UpdateHandDisplay();

        remainingActions--;
    }

    private void ActivateOrdersPhase()
    {
        remainingActions++;
        currentPhase = BattlePhase.Orders;

        if (currentTeamTurn != playerTeam)
        {
            EnemyTeamAI();
            remainingActions--;
        }

    }

    private void ActivateActionPhase()
    {
        remainingActions++;
        StartCoroutine(MinDelayPhase());
        currentPhase = BattlePhase.Action;

        TokenManager[] activeTokens = FindObjectsOfType<TokenManager>();

        float currentDelay = 0;

        foreach (TokenManager token in activeTokens)
        {

            if (token.myTeam == currentTeamTurn)
            {
                remainingActions++;
                currentDelay++;
                token.ActionPhase(currentDelay);
            }
        }

        remainingActions--;
    }

    private void ActivateCompletionPhase()
    {
        remainingActions++;
        StartCoroutine(MinDelayPhase());
        currentPhase = BattlePhase.Completion;

        TokenManager[] activeTokens = FindObjectsOfType<TokenManager>();
        foreach (TokenManager token in activeTokens)
        {

            if (token.myTeam == currentTeamTurn)
            {
                remainingActions++;
                token.CompletionPhase();
            }
        }

        remainingActions--;
    }

    public void SpawnTokenFromCard(SO_Card givenCard)
    {
        newToken = null;

        if (givenCard.cardType == CardType.Creature) { newToken = Instantiate(prefabCreatureTokenPreview, Arena.transform); }
        if (givenCard.cardType == CardType.Building) { newToken = Instantiate(prefabBuildingTokenPreview, Arena.transform); }
        if (givenCard.cardType == CardType.Spell) { newToken = Instantiate(prefabSpellTokenPreview, Arena.transform); }

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

    private void EnemyTeamAI()
    {
        if (turnCounter == waveOneSpawn) { AISpawnWavesTokens(waveOneSpawnTokens); }
        if (turnCounter == waveTwoSpawn) { AISpawnWavesTokens(waveTwoSpawnTokens); }
        if (turnCounter == waveThreeSpawn) { AISpawnWavesTokens(waveThreeSpawnTokens); }

        if ((turnCounter > waveThreeSpawn) && (turnCounter < 35)) //AI deck simulates running out of cards
        {
            int var = turnCounter - waveThreeSpawn;
            if(var % recurringWaveSpawn == 0) { AISpawnWavesTokens(recurringWaveSpawnTokens); }

        }

        TokenManager[] activeTokens = FindObjectsOfType<TokenManager>();
        float currentDelay = 0;
        foreach (TokenManager token in activeTokens)
        {

            if (token.myTeam == currentTeamTurn)
            {
                remainingActions++;
                currentDelay = currentDelay + 1.5f;
                StartCoroutine(AIStartTokenMovementAfterDelay(currentDelay, playerHQNode, token));
            }
        }
    }

    private IEnumerator AIStartTokenMovementAfterDelay(float delayTime, Node targetNode, TokenManager token)
    {
        yield return new WaitForSeconds(delayTime);
        if (battleActive) { token.MoveTokenViaAI(targetNode); }
    }

    private void AISpawnWavesTokens(List<SO_Card> toSpawn)
    {

        foreach (SO_Card tokenToSpawn in toSpawn) //random spawn location for ai tokens
        {
            List<Node> availableSpawn = FindSpawnPoints(tokenToSpawn);
            randomNodeIndex = Random.Range(0, availableSpawn.Count);
            AISpawnToken(availableSpawn[randomNodeIndex], tokenToSpawn);
        }
    }

    private List<Node> FindSpawnPoints(SO_Card toSpawnCard)
    {
        List<Node> potentialSpawns = new List<Node>();

        foreach (Node node in FindObjectOfType<ArenaGrid>().grid)
        {
            if(!node.occupied && node.walkable && node.tileTeam == otherTeam && toSpawnCard.cardType == node.tileType)
            { potentialSpawns.Add(node); }
        }

        return potentialSpawns;
    }

    private void AISpawnToken(Node spawnNode, SO_Card spawnCard)
    {
        Vector3 spawnPos = new Vector3(spawnNode.transform.position.x, spawnNode.transform.position.y + 0.5f, spawnNode.transform.position.z);
        GameObject newToken = Instantiate(prefabCreatureToken, spawnPos, Quaternion.identity);
        newToken.transform.SetParent(GameObject.Find("RedTokens").transform);
        newToken.GetComponent<TokenManager>().OnSpawn(spawnCard, Team.Red);
    }

    public void UpdateAIWaves()
    {
        waveOneSpawn = 0;
        waveOneSpawnTokens.Clear();
        waveOneSpawn = enemyDeck.enemyWaves[0].spawnWave;
        waveOneSpawnTokens.AddRange(enemyDeck.enemyWaves[0].spawnCreatures);

        waveTwoSpawn = 0;
        waveTwoSpawnTokens.Clear();
        waveTwoSpawn = enemyDeck.enemyWaves[1].spawnWave;
        waveTwoSpawnTokens.AddRange(enemyDeck.enemyWaves[1].spawnCreatures);

        waveThreeSpawn = 0;
        waveThreeSpawnTokens.Clear();
        waveThreeSpawn = enemyDeck.enemyWaves[2].spawnWave;
        waveThreeSpawnTokens.AddRange(enemyDeck.enemyWaves[2].spawnCreatures);

        recurringWaveSpawn = 0;
        recurringWaveSpawnTokens.Clear();
        recurringWaveSpawn = enemyDeck.enemyWaves[3].spawnWave;
        recurringWaveSpawnTokens.AddRange(enemyDeck.enemyWaves[3].spawnCreatures);
    }

    public void ResetBattleGameState()
    {
        //Reset Turns
        turnCounter = 0;
        remainingActions = 0;
        battleActive = false;
        currentPhase = BattlePhase.Upkeep;

        //Reset Gold
        CurrencyManager.instance.ResetCurrencyState();

        //Reset Player Deck
        PlayerDeckManager.instance.ResetDeck();

        //Reset Enemy Deck
        enemyDeck = null;

        //Reset Level
        battleLevel = null;
    }
}
