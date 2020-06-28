using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTokenManager : MonoBehaviour
{
    public SpriteRenderer myTokenImageObj;

    public GameObject prefabBuildingToken;
    public GameObject prefabCreatueToken;
    public GameObject prefabSpellToken;
    private GameObject tokenToUse;

    [SerializeField] private Sprite myTokensImage;
    [SerializeField] private SO_Card myCard;
    [SerializeField] private SO_Spell mySpellCard;

    private Vector3 snappedPos;
    private Vector3 allowedSnappedPos;
    private Node hoveredNode;

    private float yOffset;

    public void OnSpawn(SO_Card card)
    {
        myTokensImage = card.cardImage;
        myTokenImageObj.sprite = myTokensImage;
        myCard = card;
    }

    private void Update()
    {

        UpdateLocation();
        if(Input.GetMouseButtonDown(0)) { SpawnProperToken(); }
        if(hoveredNode == null) { hoveredNode = BattleManager.instance.playerHQNode; }
        if (myCard.cardType == CardType.Spell)
        {
            RemoveHighlights();
            HighlightSpellEffectArea();
        }
        if(Input.GetMouseButtonDown(1)) { ReturnTokenToHand(); }
    }

    private void UpdateLocation()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Node"))) { SnapToGrid(hitInfo.collider); }
    }

    private void SnapToGrid(Collider collisionNode)
    {
        hoveredNode = collisionNode.GetComponent<Node>();
        yOffset = 0;

        if (myCard.cardType == CardType.Creature) { yOffset = 0.5f; }
        if (myCard.cardType == CardType.Building) { yOffset = 0.75f; }
        if (myCard.cardType == CardType.Spell) { yOffset = 0.5f; }

        snappedPos = new Vector3(collisionNode.transform.position.x, collisionNode.transform.position.y + yOffset, collisionNode.transform.position.z);

        if ( 
            myCard.cardType == hoveredNode.tileType 
            && BattleManager.instance.playerTeam == hoveredNode.tileTeam 
            && !hoveredNode.occupied || myCard.cardType == CardType.Spell)
        {
            transform.position = snappedPos;
            allowedSnappedPos = snappedPos;
        }
    }

    private void SpawnProperToken()
    {

        if (myCard.cardType == CardType.Creature) { tokenToUse = prefabCreatueToken; }
        if (myCard.cardType == CardType.Building) { tokenToUse = prefabBuildingToken; }
        if (myCard.cardType == CardType.Spell) { tokenToUse = prefabSpellToken; }

        GameObject newToken = Instantiate(tokenToUse, allowedSnappedPos, Quaternion.identity);
        newToken.transform.SetParent(transform.parent);
        newToken.GetComponent<TokenManager>().OnSpawn(myCard, Team.Blue);
        CurrencyManager.instance.AlterGold(-myCard.cost);
        FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();

        if (myCard.cardType == CardType.Spell) { RemoveHighlights(); }

        Destroy(gameObject);
    }

    private void RemoveHighlights()
    {
        ArenaGrid myBattleGrid = FindObjectOfType<ArenaGrid>();
        foreach (Node node in myBattleGrid.grid) { node.RemoveHighlight(); }
    }

    private void HighlightSpellEffectArea()
    {
        List<Node> myAffectedArea = new List<Node>();
        ArenaGrid myBattleGrid = FindObjectOfType<ArenaGrid>();
        mySpellCard = myCard as SO_Spell;
        myAffectedArea = myBattleGrid.GetNeighboursInRangeVariableSize(hoveredNode, mySpellCard.xMin, mySpellCard.xMax, mySpellCard.yMin, mySpellCard.yMax);
        myAffectedArea.Add(hoveredNode);
        foreach (Node node in myAffectedArea) { node.HighlightMoveNow(); }
    }

    private void ReturnTokenToHand()
    {
        PlayerDeckManager.instance.playerHand.Add(myCard);
        FindObjectOfType<BattleArenaUIManager>().UpdateHandDisplay();
        Destroy(gameObject);
    }
}
