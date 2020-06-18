using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TokenManager : MonoBehaviour
{
    public SpriteRenderer myTokenImageObj;

    [SerializeField] protected Sprite myTokensImage;

    public SO_Card myCard = null;
    public SO_Creature myCreatureCard;
    public SO_Building myBuildingCard;
    public SO_Spell mySpellCard;

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

    public int tempRange;
    public int tempMovement;
    public int tempDamage;
    public int tempHealth;

    public List<BuffTracker> rangeBuff = new List<BuffTracker>();
    public List<BuffTracker> movementBuff = new List<BuffTracker>();
    public List<BuffTracker> damageBuff = new List<BuffTracker>();
    public List<BuffTracker> healthBuff = new List<BuffTracker>();

    public int totalRange;
    public int totalMovement;
    public int totalDamage;
    public int totalHealth;

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
        if(myCard.cardType == CardType.Spell && mySpellCard != null)
        {
            RunSpellFunction();
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

        if (myTokenImageObj != null) { myTokenImageObj.sprite = myTokensImage; }

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

            GetComponent<MeshRenderer>().material = TokenMaterial();
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

            GetComponent<MeshRenderer>().material = TokenMaterial();

        }

        if (myCard.cardType == CardType.Spell)
        {
            mySpellCard = myCard as SO_Spell;
        }

        RecalcBuffs();

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
        int plannedMovement = 0;

        if (myPath.Count > remainingMovement) { plannedMovement = remainingMovement; } else { plannedMovement = myPath.Count; }
        float delayPerStep = 0.75f / plannedMovement;

        for (int i = 0; i < remainingMovement; i++)
        {

            if (i < myPath.Count)
            {
                Vector3 tilePosition = new Vector3(myPath[i].transform.position.x, myPath[i].transform.position.y + 0.5f, myPath[i].transform.position.z);
                transform.position = tilePosition;
                usedMovement++;
                yield return new WaitForSeconds(delayPerStep);
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
        if(BattleManager.instance.currentTeamTurn == myTeam)
        {
        foreach (BuffTracker buff in rangeBuff.ToArray()) { buff.turnsRemaining --; if (buff.turnsRemaining < 1) { rangeBuff.Remove(buff); } }
        foreach (BuffTracker buff in movementBuff.ToArray()) {  buff.turnsRemaining--;Debug.Log("Buff in Array : Imapct" + buff.impact + " & Turns remaining : " + buff.turnsRemaining); if (buff.turnsRemaining < 1) { movementBuff.Remove(buff); } }
        foreach (BuffTracker buff in damageBuff.ToArray()) { buff.turnsRemaining--; if (buff.turnsRemaining < 1) { damageBuff.Remove(buff); } }
        foreach (BuffTracker buff in healthBuff.ToArray()) { buff.turnsRemaining--; if (buff.turnsRemaining < 1) { healthBuff.Remove(buff); } }
        }

        RecalcBuffs();
        
        remainingMovement = totalMovement;
    }

    public virtual void ActionPhase(float givenDelay)
    {
        actionRemaining = true;
        
        // Search for enemy tokens within X squares
        List<Node> nodesInRange = myBattleGrid.GetNeighboursInRange(ReturnNodeIAmOn(), totalRange);
        List<TokenManager> enemiesInRange = new List<TokenManager>();
        foreach (Node node in nodesInRange) { if(node.occupied) { if (node.ReturnTokenOnNode().myTeam != myTeam) { enemiesInRange.Add(node.ReturnTokenOnNode()); } } }

        // Order the enemies based off a value (lowest health first)
        enemiesInRange.Sort(Utils.SortByHealth);

        foreach (TokenManager token in enemiesInRange) { StartCoroutine(DealDamageAfterTime(totalDamage, givenDelay, token)); }

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

    protected virtual IEnumerator DealDamageAfterTime(int damage, float currentDelay, TokenManager enemyToBeAttacked)
    {
        BattleManager.instance.remainingActions++;

        yield return new WaitForSeconds(currentDelay + 0.75f);

        if(enemyToBeAttacked != null)
        {
            enemyToBeAttacked.TakeDamage(damage, true, this);
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
        currentHealth = Mathf.Clamp(currentHealth + val, 0, totalHealth);
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

    private void RunSpellFunction()
    {
        ClearPathHighlights();

        if (mySpellCard.spellType == SpellType.Damage)
        {
            //Find nodes in affected area
            List<Node> myAffectedArea = new List<Node>();
            ArenaGrid myBattleGrid = FindObjectOfType<ArenaGrid>();
            mySpellCard = myCard as SO_Spell;
            myAffectedArea = myBattleGrid.GetNeighboursInRangeVariableSize(ReturnNodeIAmOn(), mySpellCard.xMin, mySpellCard.xMax, mySpellCard.yMin, mySpellCard.yMax);
            myAffectedArea.Add(ReturnNodeIAmOn());

            //Check for enemies on each node
            List<TokenManager> enemiesInRange = new List<TokenManager>();
            foreach (Node node in myAffectedArea)
            {
                if (node.occupied)
                {
                    if (node.ReturnTokenOnNode().myTeam != myTeam)
                    {
                        enemiesInRange.Add(node.ReturnTokenOnNode());
                    }
                }
            }

            //Deal Damage to them
            foreach (TokenManager token in enemiesInRange)
            {
                StartCoroutine(DealDamageAfterTime(mySpellCard.damageAmount, 0, token));
            }
        }

        if (mySpellCard.spellType == SpellType.Buff)
        {
            //Find nodes in affected area
            List<Node> myAffectedArea = new List<Node>();
            ArenaGrid myBattleGrid = FindObjectOfType<ArenaGrid>();
            mySpellCard = myCard as SO_Spell;
            myAffectedArea = myBattleGrid.GetNeighboursInRangeVariableSize(ReturnNodeIAmOn(), mySpellCard.xMin, mySpellCard.xMax, mySpellCard.yMin, mySpellCard.yMax);
            myAffectedArea.Add(ReturnNodeIAmOn());

            //Check for allies on each node
            List<TokenManager> alliesInRange = new List<TokenManager>();
            foreach (Node node in myAffectedArea)
            {
                if (node.occupied)
                {
                    if (node.ReturnTokenOnNode().myTeam == myTeam)
                    {
                        alliesInRange.Add(node.ReturnTokenOnNode());
                    }
                }
            }

            foreach (TokenManager ally in alliesInRange)
            {
                Debug.Log("Detected Token : " + ally.name);

                if (mySpellCard.rangeBuff != 0) { ally.AddBuff(StatType.Range, mySpellCard.rangeBuff, mySpellCard.buffLength); Debug.Log("Added Buff : Range"); }
                if (mySpellCard.movementBuff != 0) { ally.AddBuff(StatType.Movement, mySpellCard.movementBuff, mySpellCard.buffLength); Debug.Log("Added Buff : Movement"); }
                if (mySpellCard.attackBuff != 0) { ally.AddBuff(StatType.Attack, mySpellCard.attackBuff, mySpellCard.buffLength); Debug.Log("Added Buff : Attack"); }
                if (mySpellCard.healthBuff != 0) { ally.AddBuff(StatType.Health, mySpellCard.healthBuff, mySpellCard.buffLength); Debug.Log("Added Buff : Health"); }

                ally.RecalcBuffs();
            }

        }

        if (mySpellCard.spellType == SpellType.Debuff)
        {
            //Find nodes in affected area
            List<Node> myAffectedArea = new List<Node>();
            ArenaGrid myBattleGrid = FindObjectOfType<ArenaGrid>();
            mySpellCard = myCard as SO_Spell;
            myAffectedArea = myBattleGrid.GetNeighboursInRangeVariableSize(ReturnNodeIAmOn(), mySpellCard.xMin, mySpellCard.xMax, mySpellCard.yMin, mySpellCard.yMax);
            myAffectedArea.Add(ReturnNodeIAmOn());

            //Check for enemies on each node
            List<TokenManager> enemiesInRange = new List<TokenManager>();
            foreach (Node node in myAffectedArea)
            {
                if (node.occupied)
                {
                    if (node.ReturnTokenOnNode().myTeam != myTeam)
                    {
                        enemiesInRange.Add(node.ReturnTokenOnNode());
                    }
                }
            }

            foreach (TokenManager enemy in enemiesInRange)
            {
                if (mySpellCard.rangeBuff != 0) { enemy.AddBuff(StatType.Range, mySpellCard.rangeBuff, mySpellCard.buffLength); Debug.Log("Added DeBuff : Range"); }
                if (mySpellCard.movementBuff != 0) { enemy.AddBuff(StatType.Movement, mySpellCard.movementBuff, mySpellCard.buffLength); Debug.Log("Added DeBuff : Movement"); }
                if (mySpellCard.attackBuff != 0) { enemy.AddBuff(StatType.Attack, mySpellCard.attackBuff, mySpellCard.buffLength); Debug.Log("Added DeBuff : Attack"); }
                if (mySpellCard.healthBuff != 0) { enemy.AddBuff(StatType.Health, mySpellCard.healthBuff, mySpellCard.buffLength); Debug.Log("Added DeBuff : Health"); }

                enemy.RecalcBuffs();
            }

        }

        if (mySpellCard.spellType == SpellType.Resource)
        {
            if (mySpellCard.goldPerAllyCreature > 0)
            {
                TokenManager[] activeTokens = FindObjectsOfType<TokenManager>();
                foreach (TokenManager token in activeTokens)
                {
                    if (token.myTeam == myTeam && token.myCreatureCard != null)
                    {
                        CurrencyManager.instance.AlterGold(mySpellCard.goldPerAllyCreature);
                        FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();
                    }
                }
            }
        }

            Destroy(gameObject);

    }

    public void AddBuff(StatType statToBuff, int valToBuff, int turnsToBuff)
    {
        BuffTracker newBuff = new BuffTracker();

        newBuff.turnsRemaining = turnsToBuff;
        newBuff.impact = valToBuff;

        switch(statToBuff)
        {
            case StatType.Range:
                rangeBuff.Add(newBuff);
                break;

            case StatType.Movement:
                movementBuff.Add(newBuff);
                remainingMovement = remainingMovement + newBuff.impact;
                break;

            case StatType.Attack:
                damageBuff.Add(newBuff);
                break;

            case StatType.Health:
                healthBuff.Add(newBuff);
                break;
        }
    }

    public void RecalcBuffs()
    {
        tempRange = 0;
        foreach (BuffTracker buff in rangeBuff) { tempRange = tempRange + buff.impact; }
        totalRange = attackRange + tempRange;

        tempMovement = 0;
        foreach (BuffTracker buff in movementBuff) { tempMovement = tempMovement + buff.impact; }
        totalMovement = maxMovement + tempMovement;

        tempDamage = 0;
        foreach (BuffTracker buff in damageBuff) { tempDamage = tempDamage + buff.impact; }
        totalDamage = attackDamage + tempDamage;

        tempHealth = 0;
        foreach (BuffTracker buff in healthBuff) { tempHealth = tempHealth + buff.impact; }
        totalHealth = maxHealth + tempHealth;
    }
}
