using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int xRow;
    public int yRow;

    public CardType tileType;
    public Team tileTeam;

    public bool occupied { get { return CheckForOccupied(); } }
    public bool walkable;

    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    public Node parentNode;

    private Material baseMaterial;
    public Material canMoveMaterial;
    public Material canMoveLaterMaterial;

    public int pathfinderDistanceToNode;

    private void Start()
    {
        UpdateMaterial(tileTeam, tileType, walkable);
        canMoveMaterial = MaterialsManager.instance.nodeWalkNow;
        canMoveLaterMaterial = MaterialsManager.instance.nodeWalkLater;
        GetComponent<MeshRenderer>().material = baseMaterial;
    }

    private bool CheckForOccupied()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, Vector3.up);
        if (Physics.Raycast(ray, out hitInfo, 2, LayerMask.GetMask("Token"))) { return true; } else { return false; }
    }

    public TokenManager ReturnTokenOnNode()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, Vector3.up);
        if (Physics.Raycast(ray, out hitInfo, 2, LayerMask.GetMask("Token"))) { return hitInfo.collider.GetComponent<TokenManager>(); }
        return new TokenManager();
    }

    public void HighlightMoveNow() { GetComponent<MeshRenderer>().material = canMoveMaterial; }
    public void HighlightMoveLater() { GetComponent<MeshRenderer>().material = canMoveLaterMaterial; }
    public void RemoveHighlight() { GetComponent<MeshRenderer>().material = baseMaterial; }

    public void UpdateMaterial(Team myNodesTeam, CardType cardType, bool walkable)
    {
        if (walkable)
        {
            if (myNodesTeam == Team.Blue)
            {
                if (cardType == CardType.Creature) { baseMaterial = MaterialsManager.instance.nodeBlueCreature; }
                if (cardType == CardType.Building) { baseMaterial = MaterialsManager.instance.nodeBlueBuilding; }
            }
            else if (myNodesTeam == Team.Red)
            {
                if (cardType == CardType.Creature) { baseMaterial = MaterialsManager.instance.nodeRedCreature; }
                if (cardType == CardType.Building) { baseMaterial = MaterialsManager.instance.nodeRedBuilding; }
            }
            else if (myNodesTeam == Team.None)
            { baseMaterial = MaterialsManager.instance.nodeBasic; }
        }
        else
        {
            baseMaterial = MaterialsManager.instance.nodeUnwalkable;
        }

        GetComponent<MeshRenderer>().material = baseMaterial;
    }

}
