using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenManager : MonoBehaviour
{
    public SpriteRenderer myTokenImageObj;

    [SerializeField] private Sprite myTokensImage;

    [SerializeField] private SO_Card myCard;
    private SO_Creature myCreatureCard;

    public Team myTeam;

    [SerializeField] private Material baseTokenMaterial = null;

    [SerializeField] private Material blueTeamCreature = null;
    [SerializeField] private Material blueTeamBuilding = null;

    [SerializeField] private Material redTeamCreature = null;
    [SerializeField] private Material redTeamBuilding = null;

    private int maxMovement;
    private int remainingMovement;

    public Node seekerNode;
    public Node targetNode;

    public bool movementPreview;

    private ArenaGrid myBattleGrid;
    private List<Node> myPath = new List<Node>();
    public int clicks = 0;

    private void Start()
    {
        myBattleGrid = FindObjectOfType<ArenaGrid>();
        if(myCard != null) { TokenSetUp(); }
    }

    private void Update()
    {
        if (movementPreview && ReturnNodeUnderMouse() != null)
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

    public void OnSpawn(SO_Card givenCard, Team givenTeam)
    {
        myCard = givenCard;
        TokenSetUp();
        myTeam = givenTeam;
    }

    private void TokenSetUp()
    {
        myTokensImage = myCard.cardImage;
        myTokenImageObj.sprite = myTokensImage;

        if (myCard.cardType == CardType.Creature)
        {
            myCreatureCard = myCard as SO_Creature;
            maxMovement = myCreatureCard.movement;
            remainingMovement = 0;
        }

        GetComponent<MeshRenderer>().material = TokenMaterial();
    }

    private Material TokenMaterial()
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

    private Node ReturnNodeUnderMouse()
    {
        Node mouseNode = null;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Node"))) { mouseNode = hitInfo.collider.GetComponent<Node>(); }
        return mouseNode;
    }

    private Node ReturnNodeIAmOn()
    {
        Node currentNode = null;
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Node"))) { currentNode = hitInfo.collider.GetComponent<Node>(); }
        return currentNode;
    }

    public void MoveToken()
    {
        Debug.Log("Confirmed Movement");
        foreach (Node node in myBattleGrid.grid) { node.RemoveHighlight(); }
        movementPreview = !movementPreview;
        StartCoroutine(MoveTokenOverTime());
    }

    private IEnumerator MoveTokenOverTime()
    {
        int usedMovement = 0;

        for (int i = 0; i < remainingMovement; i++)
        {
            if(i < myPath.Count)
            {
                Vector3 tilePosition = new Vector3(myPath[i].transform.position.x, myPath[i].transform.position.y + 0.5f, myPath[i].transform.position.z);
                transform.position = tilePosition;
                usedMovement++;
                yield return new WaitForSeconds(0.5f);
            }
        }

        remainingMovement = remainingMovement - usedMovement;

    }

    public void ClearPathHighlights()
    {
        foreach (Node node in myBattleGrid.grid) { node.RemoveHighlight(); }
    }

    public void UpkeepPhase()
    {
        remainingMovement = maxMovement; 
    }

    public void ActionPhase()
    {


        Debug.Log(this.name + "IN ACTION PHASE");

        BattleManager.instance.remainingActions--;
    }

}
