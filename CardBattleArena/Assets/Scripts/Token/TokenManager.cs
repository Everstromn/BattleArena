using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TokenManager : MonoBehaviour
{
    public SpriteRenderer myTokenImageObj;

    [SerializeField] protected Sprite myTokensImage;

    public SO_Card myCard;
    public SO_Creature myCreatureCard;

    public Team myTeam;

    [SerializeField] protected Material baseTokenMaterial = null;

    [SerializeField] protected Material blueTeamCreature = null;
    [SerializeField] protected Material blueTeamBuilding = null;

    [SerializeField] protected Material redTeamCreature = null;
    [SerializeField] protected Material redTeamBuilding = null;

    protected int maxMovement;
    protected int remainingMovement;

    public Node seekerNode;
    public Node targetNode;

    public bool movementPreview;

    protected ArenaGrid myBattleGrid;
    protected List<Node> myPath = new List<Node>();
    public int clicks = 0;

    protected int attackRange;
    protected int attackDamage;

    public int maxHealth;
    public int currentHealth = 1;

    [SerializeField] protected GameObject damageDispalyObj = null;
    [SerializeField] protected TMP_Text damageDisplayTextObj = null;

    protected virtual void Start()
    {
        myBattleGrid = FindObjectOfType<ArenaGrid>();
        if(myCard != null) { TokenSetUp(); }
    }

    protected virtual void Update()
    {
        if (movementPreview && ReturnNodeUnderMouse() != null && myTeam == BattleManager.instance.playerTeam)
        {
            seekerNode = ReturnNodeIAmOn();
            targetNode = ReturnNodeUnderMouse();
            myPath = GetComponent<PathFinder>().FindPath(seekerNode, targetNode);
            ClearPathHighlights();
            for (int i = 0; i < myPath.Count; i++)
            {
                if(i < remainingMovement) { myPath[i].HighlightMoveNow(); } else { myPath[i].HighlightMoveLater(); }
            }
        }
    }

    public virtual void OnSpawn(SO_Card givenCard, Team givenTeam)
    {
        myCard = givenCard;
        TokenSetUp();
        myTeam = givenTeam;
    }

    protected virtual void TokenSetUp()
    {
        myTokensImage = myCard.cardImage;
        myTokenImageObj.sprite = myTokensImage;

        if (myCard.cardType == CardType.Creature)
        {
            myCreatureCard = myCard as SO_Creature;

            maxHealth = myCreatureCard.health;
            currentHealth = maxHealth;

            maxMovement = myCreatureCard.movement;
            remainingMovement = 0;

            attackRange = myCreatureCard.attackRange;
            attackDamage = myCreatureCard.damage;
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
        Debug.Log("Confirmed Movement");
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
        // Search for enemy tokens within X squares
        List<Node> nodesInRange = myBattleGrid.GetNeighboursInRange(ReturnNodeIAmOn(), attackRange);
        List<TokenManager> enemiesInRange = new List<TokenManager>();

        foreach (Node node in nodesInRange)
        {
            if(node.occupied) { if (node.ReturnTokenOnNode().myTeam != myTeam) { enemiesInRange.Add(node.ReturnTokenOnNode()); } }
        }

        // Order the enemies based off a value (lowest health first)
        enemiesInRange.Sort(Utils.SortByHealth);

        foreach (TokenManager token in enemiesInRange)
        {
            StartCoroutine(DealDamageAfterTime(givenDelay, token));
        }

        BattleManager.instance.remainingActions--;
    }

    protected virtual IEnumerator DealDamageAfterTime(float currentDelay, TokenManager enemnyToBeAttacked)
    {
        BattleManager.instance.remainingActions++;

        yield return new WaitForSeconds(currentDelay + 0.75f);

        Debug.Log(this.name + " is dealing damage to enemy : " + enemnyToBeAttacked.name);
        enemnyToBeAttacked.TakeDamage(attackDamage);
        BattleManager.instance.remainingActions--;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        StartCoroutine(ActivateDamageDisplay(damage));
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
        targetNode = ReturnClosestToNode(givenTargetNode);
        myPath = GetComponent<PathFinder>().FindPath(seekerNode, targetNode);
        StartCoroutine(MoveTokenOverTime());
        BattleManager.instance.remainingActions--;
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
}
