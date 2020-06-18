using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; //added for rightclick preview pucks, using IPointerClickHandler

public class TokenManager : MonoBehaviour, IPointerClickHandler
{
    public bool preview = false; // for rightclick preview


    public SpriteRenderer myTokenImageObj;

    [SerializeField] protected Sprite myTokensImage;

    public SO_Card myCard = null;
    public SO_Creature myCreatureCard;
    public SO_Building myBuildingCard;

    public Team myTeam;

    protected MeshRenderer myMeshRenderer;

    [SerializeField] protected Material baseTokenMaterial = null;

    [SerializeField] protected Material blueTeamCreature = null;
    [SerializeField] protected Material blueTeamCreatureAction = null;
    [SerializeField] protected Material blueTeamBuilding = null;

    [SerializeField] protected Material redTeamCreature = null;
    [SerializeField] protected Material redTeamCreatureAction = null;
    [SerializeField] protected Material redTeamBuilding = null;

    protected bool emission = false;
    protected bool actionRemaining = false;
    public GameObject actionPuck;
    public GameObject prefabCreatureToken;

    protected int maxMovement;
    protected int remainingMovement;

    public Node seekerNode;
    public Node targetNode;

    public bool movementPreview;

    protected ArenaGrid myBattleGrid;
    protected List<Node> myPath = new List<Node>();
    protected List<Node> myBuildPath = new List<Node>();
    public int clicks = 0;

    protected int attackRange;
    protected int attackDamage;

    public int maxHealth;
    public int currentHealth = 1;

    [SerializeField] protected GameObject damageDispalyObj = null;
    [SerializeField] protected TMP_Text damageDisplayTextObj = null;

    protected bool haveSetUp = false;

    protected int countToAction;

    protected virtual void Start()
    {
        myBattleGrid = FindObjectOfType<ArenaGrid>();
        myMeshRenderer = GetComponent<MeshRenderer>();

        if (!haveSetUp && myCard != null) { TokenSetUp(); }
    }

    protected virtual void Update()
    {
        if(myCard.cardType == CardType.Creature)
        {
            if (movementPreview && ReturnNodeUnderMouse() != null && myTeam == BattleManager.instance.playerTeam)
            {
                if(BattleManager.instance.currentTeamTurn == BattleManager.instance.playerTeam)
                {
                    seekerNode = ReturnNodeIAmOn();
                    targetNode = ReturnNodeUnderMouse();
                    myPath = GetComponent<PathFinder>().FindPath(seekerNode, targetNode);
                    ClearPathHighlights();
                    for (int i = 0; i < myPath.Count; i++)
                    {
                        if (i < remainingMovement) { myPath[i].HighlightMoveNow(); } else { myPath[i].HighlightMoveLater(); }
                    }
                }
            }

            if (BattleManager.instance.currentPhase == BattlePhase.Orders)
            {
                if (remainingMovement > 0 && myTeam == BattleManager.instance.currentTeamTurn)
                {
                    actionPuck.SetActive(true);
                }
                else
                {
                    actionPuck.SetActive(false);
                }
            }

            if (BattleManager.instance.currentPhase == BattlePhase.Action)
            {
                if (actionRemaining&& myTeam == BattleManager.instance.currentTeamTurn)
                {
                    actionPuck.SetActive(true);
                }
                else
                {
                    actionPuck.SetActive(false);
                }
            }

            if (BattleManager.instance.currentPhase != BattlePhase.Action && BattleManager.instance.currentPhase != BattlePhase.Orders)
            {
                if (actionPuck.activeSelf)
                {
                    actionPuck.SetActive(false);
                }
            }

        }
    }
    // preview when rightclicking puck
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("detectedclick");
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!preview)
            {
                FindObjectOfType<BattleArenaUIManager>().EnableCardPreview(myCard);
            }
            else
            {
                FindObjectOfType<BattleArenaUIManager>().DisableCardPreview();
            }
        }
    }

    public virtual void OnSpawn(SO_Card givenCard, Team givenTeam)
    {
        myCard = givenCard;
        myTeam = givenTeam;
        if (!haveSetUp) { TokenSetUp(); }
    }

    protected virtual void TokenSetUp()
    {
        haveSetUp = true;        
        myTokensImage = myCard.cardImage;
        myTokenImageObj.sprite = myTokensImage;

        if (myCard.cardType == CardType.Creature)
        {
            myCreatureCard = myCard as SO_Creature;

            maxHealth = myCreatureCard.health;
            currentHealth = maxHealth;

            maxMovement = myCreatureCard.movement;
            if(myCreatureCard.hasHaste)
            {
                remainingMovement = maxMovement;
            }
            else
            {
                remainingMovement = 0;
            }

            attackRange = myCreatureCard.attackRange;
            attackDamage = myCreatureCard.damage;
        }

        if (myCard.cardType == CardType.Building)
        {
            myBuildingCard= myCard as SO_Building;

            maxHealth = myBuildingCard.health;
            currentHealth = maxHealth;

            attackRange = myBuildingCard.attackRange;
            attackDamage = myBuildingCard.damage;

            if(myBuildingCard.impactsGoldPerTurn) { CurrencyManager.instance.IncreaseGoldPerTurn(myBuildingCard.goldPerTurnIncrease); }
            if(myBuildingCard.canSpawnToken) { countToAction = myBuildingCard.tokenSpawnEveryXTturns; }
            if(myBuildingCard.canChangeNodeType)
            {
                List<Node> changeNodes = new List<Node>();
                Node myNode = ReturnNodeIAmOn();

                myBattleGrid = FindObjectOfType<ArenaGrid>();
                changeNodes = myBattleGrid.GetNeighboursInRange(myNode, 1);

                foreach (Node node in changeNodes)
                {
                    node.tileType = myBuildingCard.nodeTypeToAdd;
                    node.tileTeam = myTeam;
                    node.UpdateMaterial(node.tileTeam, node.tileType, node.walkable);
                }
            }
        }

        GetComponent<MeshRenderer>().material = TokenMaterial();

    }

    protected virtual Material TokenMaterial()
    {
        if(myTeam == Team.Blue)
        {
            if (myCard.cardType == CardType.Creature) { return blueTeamCreature; } 
            if (myCard.cardType == CardType.Building) { return blueTeamBuilding; }
        }
        else if (myTeam == Team.Red)
        {
            if (myCard.cardType == CardType.Creature) { return redTeamCreature; }
            if (myCard.cardType == CardType.Building) { return redTeamBuilding; }
        }

        return baseTokenMaterial;
    }

    protected virtual Node ReturnNodeUnderMouse()
    {
        Node mouseNode = null;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Node"))) { mouseNode = hitInfo.collider.GetComponent<Node>(); }
        return mouseNode;
    }

    protected virtual Node ReturnNodeIAmOn()
    {
        Node currentNode = null;
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Node"))) { currentNode = hitInfo.collider.GetComponent<Node>(); }
        return currentNode;
    }

    public virtual void MoveToken()
    {
        actionPuck.GetComponent<MeshRenderer>().material.color = Color.grey;
        foreach (Node node in myBattleGrid.grid) { node.RemoveHighlight(); }
        movementPreview = !movementPreview;
        StartCoroutine(MoveTokenOverTime());
    }

    protected virtual IEnumerator MoveTokenOverTime()
    {
        int usedMovement = 0;
        for (int i = 0; i < remainingMovement; i++)
        {

            if (i < myPath.Count)
            {
                Vector3 tilePosition = new Vector3(myPath[i].transform.position.x, myPath[i].transform.position.y + 0.5f, myPath[i].transform.position.z);
                transform.position = tilePosition;
                usedMovement++;
                yield return new WaitForSeconds(0.5f);
            }
        }

        remainingMovement = remainingMovement - usedMovement;

    }

    public virtual void ClearPathHighlights()
    {
        foreach (Node node in myBattleGrid.grid) { node.RemoveHighlight(); }
    }

    public virtual void UpkeepPhase()
    {
        remainingMovement = maxMovement;
    }

    public virtual void ActionPhase(float givenDelay)
    {
        actionRemaining = true;
        
        // Search for enemy tokens within X squares
        List<Node> nodesInRange = myBattleGrid.GetNeighboursInRange(ReturnNodeIAmOn(), attackRange);
        List<TokenManager> enemiesInRange = new List<TokenManager>();
        foreach (Node node in nodesInRange) { if(node.occupied) { if (node.ReturnTokenOnNode().myTeam != myTeam) { enemiesInRange.Add(node.ReturnTokenOnNode()); } } }

        // Order the enemies based off a value (lowest health first)
        enemiesInRange.Sort(Utils.SortByHealth);

        foreach (TokenManager token in enemiesInRange) { StartCoroutine(DealDamageAfterTime(givenDelay, token)); }

        if (myCard.cardType == CardType.Building)
        {
            if(myBuildingCard.canSpawnToken)
            {
                if(countToAction > 0)
                {
                    countToAction--;
                }
                else
                {
                    SpawnTokenFromBuilding();
                    countToAction = myBuildingCard.tokenSpawnEveryXTturns;
                }
            }
        }

        BattleManager.instance.remainingActions--;
    }

    protected virtual IEnumerator DealDamageAfterTime(float currentDelay, TokenManager enemyToBeAttacked)
    {
        BattleManager.instance.remainingActions++;

        yield return new WaitForSeconds(currentDelay + 0.75f);

        if(enemyToBeAttacked != null)
        {
            enemyToBeAttacked.TakeDamage(attackDamage, true, this);
            actionRemaining = false;
        }
        BattleManager.instance.remainingActions--;
    }

    public virtual void TakeDamage(int damage, bool retailiate, TokenManager agressor)
    {
        currentHealth = currentHealth - damage;

        if (retailiate && myCard != null)
        {
            if (myCard.cardType == CardType.Creature)
            {
                agressor.TakeDamage(myCreatureCard.retaliationDamage, false, this);
            }
        }

        if(damage > 0)
        {
            StartCoroutine(ActivateDamageDisplay(damage));
        }
    }

    protected virtual IEnumerator ActivateDamageDisplay(int damage)
    {
        damageDispalyObj.SetActive(true);
        damageDisplayTextObj.text = damage.ToString();

        yield return new WaitForSeconds(0.5f);

        damageDispalyObj.SetActive(false);


        if (currentHealth <= 0) { Death(); }
    }

    protected virtual void Death()
    {
        if (myCard.cardType == CardType.Building)
        {
            if (myBuildingCard.impactsGoldPerTurn)
            {
                CurrencyManager.instance.IncreaseGoldPerTurn(-myBuildingCard.goldPerTurnIncrease);
                FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();
            }
        }

            Destroy(gameObject);
    }

    protected virtual void OnMouseOver()
    {
        FindObjectOfType<BattleArenaUIManager>().DisplayHover(this);
    }

    protected virtual void OnMouseExit()
    {
        FindObjectOfType<BattleArenaUIManager>().HideHover();
    }

    public virtual void MoveTokenViaAI(Node givenTargetNode)
    {
        seekerNode = ReturnNodeIAmOn();

        if(myBattleGrid.GetDistance(seekerNode, givenTargetNode) > myCreatureCard.attackRange)
        {
            targetNode = ReturnClosestToNode(givenTargetNode);
            myPath = GetComponent<PathFinder>().FindPath(seekerNode, targetNode);
            StartCoroutine(MoveTokenOverTime());
            BattleManager.instance.remainingActions--;
        }
    }

    private Node ReturnClosestToNode(Node target)
    {
        Node selectedNode = target;
        List<Node> selectedNodesNeighbours = myBattleGrid.GetNeighbours(selectedNode);

        while (selectedNode.occupied)
        {
            selectedNodesNeighbours.Remove(selectedNode);
            foreach (Node node in selectedNodesNeighbours) { node.pathfinderDistanceToNode = myBattleGrid.GetDistance(node, target); }
            selectedNodesNeighbours.Sort(Utils.SortByDistanceToNode);            
            selectedNode = selectedNodesNeighbours[0];
        }

        return selectedNode;
    }

    public virtual void CompletionPhase()
    {
        if(myCreatureCard != null)
        {
            if(myCreatureCard.regenerates) { GainHealth(myCreatureCard.regenerationAmount); }
        }

        BattleManager.instance.remainingActions--;
    }

    protected virtual void GainHealth(int val)
    {
        currentHealth = Mathf.Clamp(currentHealth + val, 0, maxHealth);
    }

    private void SpawnToken(Node spawnNode, SO_Card spawnCard, Team givenTeam)
    {
        Vector3 spawnPos = new Vector3(spawnNode.transform.position.x, spawnNode.transform.position.y + 0.5f, spawnNode.transform.position.z);
        GameObject newToken = Instantiate(prefabCreatureToken, spawnPos, Quaternion.identity);
        newToken.transform.SetParent(GameObject.Find("RedTokens").transform);
        newToken.GetComponent<TokenManager>().OnSpawn(spawnCard, givenTeam);
    }

    private void SpawnTokenFromBuilding()
    {
        targetNode = BattleManager.instance.AIHQNode;
        seekerNode = ReturnNodeIAmOn();

        myBuildPath = GetComponent<PathFinder>().FindPath(seekerNode, targetNode);

        if (myBuildPath.Count > 0) { Debug.Log("Found Path"); } else { Debug.Log("Found No Path"); }

        SpawnToken(myBuildPath[0], myBuildingCard.tokenToSpawn, BattleManager.instance.currentTeamTurn);
    }
}
