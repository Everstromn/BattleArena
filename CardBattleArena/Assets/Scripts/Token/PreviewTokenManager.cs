using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTokenManager : MonoBehaviour
{
    public SpriteRenderer myTokenImageObj;
    public GameObject prefabToken;

    [SerializeField] private Sprite myTokensImage;
    [SerializeField] private SO_Card myCard;

    private Vector3 snappedPos;
    private Node hoveredNode;

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
        snappedPos = new Vector3(collisionNode.transform.position.x, collisionNode.transform.position.y + 0.5f, collisionNode.transform.position.z);
        if ( 
            myCard.cardType == hoveredNode.tileType 
            && BattleManager.instance.playerTeam == hoveredNode.tileTeam 
            && !hoveredNode.occupied)
        {
            transform.position = snappedPos;
        }
    }

    private void SpawnProperToken()
    {
        GameObject newToken = Instantiate(prefabToken, snappedPos, Quaternion.identity);
        newToken.transform.SetParent(transform.parent);
        newToken.GetComponent<TokenManager>().OnSpawn(myCard, Team.Blue);
        CurrencyManager.instance.AlterGold(-myCard.cost);
        FindObjectOfType<BattleArenaUIManager>().UpdateGoldDisplay();
        Destroy(gameObject);
    }
}
