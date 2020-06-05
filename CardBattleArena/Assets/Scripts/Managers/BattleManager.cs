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

    public bool battleActive;
    [SerializeField] private float battleStartDelay = 7;

    public GameObject prefabToken;
    public GameObject prefabTokenPreview;
    public GameObject Arena;
    public BattlePhase currentPhase;
    public Team currentTeamTurn;

    public Team playerTeam;
    public Color playerTeamColor = Color.blue;
    public Team otherTeam;
    public Color otherTeamColor = Color.red;

    public Node playerHQNode;

    public int playerStartingHealth;

    public int remainingActions = 0;
    public int turnCounter = 0;

    public int waveOneSpawn = 3;
    public List<SO_Card> waveOneSpawnTokens = new List<SO_Card>();

    public int waveTwoSpawn = 3;
    public List<SO_Card> waveTwoSpawnTokens = new List<SO_Card>();

    public float minPhaseTime = 0.5f;

    private void Start()
    {
        FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();
        battleActive = false;
        StartCoroutine(BattleInitiationDelay());
    }

    private IEnumerator BattleInitiationDelay()
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

        if (currentTeamTurn == playerTeam)
        {
            turnCounter++;
            CurrencyManager.instance.IncreaseGoldPerTurn(1);
            CurrencyManager.instance.AddTurnGold();
            FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();

        }

        RefreshAllTokens();

        remainingActions--;
    }

    private void ActivateDrawPhase()
    {
        remainingActions++;
        StartCoroutine(MinDelayPhase());
        currentPhase = BattlePhase.Draw;

        if (currentTeamTurn == playerTeam)
        {
            PlayerDeckManager.instance.DrawCardFromDeck();
            FindObjectOfType<BattleArenaUIManager>().UpdateHandDisplay();
        }

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

    private void EnemyTeamAI()
    {
        if (turnCounter == waveOneSpawn) { AISpawnWavesTokens(waveOneSpawnTokens); }
        if (turnCounter == waveTwoSpawn) { AISpawnWavesTokens(waveTwoSpawnTokens); }

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
        token.MoveTokenViaAI(targetNode);
    }

    private void AISpawnWavesTokens(List<SO_Card> toSpawn)
    {

        foreach (SO_Card tokenToSpawn in toSpawn)
        {
            List<Node> availableSpawn = FindSpawnPoints(tokenToSpawn);
            AISpawnToken(availableSpawn[0], tokenToSpawn);
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
        GameObject newToken = Instantiate(prefabToken, spawnPos, Quaternion.identity);
        newToken.transform.SetParent(GameObject.Find("RedTokens").transform);
        newToken.GetComponent<TokenManager>().OnSpawn(spawnCard, Team.Red);
    }
}
