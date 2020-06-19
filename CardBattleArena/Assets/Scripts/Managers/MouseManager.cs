using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private TokenManager selectedToken;

    private void Update()
    {
        if(BattleManager.instance.battleActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<BattleArenaUIManager>().DisableCardPreview();

                if (selectedToken != null && ReturnTokenUnderMouse() == null)
                {
                    if(ReturnNodeUnderMouse() != null)
                    {
                        selectedToken.MoveToken();
                        selectedToken.actionPuck.GetComponent<MeshRenderer>().material.color = Color.grey;
                        selectedToken = null;
                    }
                    else
                    {
                        selectedToken.movementPreview = false;
                        selectedToken.ClearPathHighlights();
                        selectedToken.actionPuck.GetComponent<MeshRenderer>().material.color = Color.grey;
                        selectedToken = null;
                    }

                }

                if (selectedToken == null && ReturnTokenUnderMouse() != null && BattleManager.instance.currentTeamTurn == BattleManager.instance.playerTeam)
                {
                    if (ReturnTokenUnderMouse().myCard != null)
                    {
                        if (ReturnTokenUnderMouse().myCard.cardType == CardType.Creature)
                        {
                            selectedToken = ReturnTokenUnderMouse();
                            selectedToken.movementPreview = true;
                        }
                    }
                }
            }

            else if(Input.GetMouseButtonDown(1))
            {
                if (selectedToken != null)
                {
                    selectedToken.movementPreview = false;
                    selectedToken.ClearPathHighlights();
                    selectedToken.actionPuck.GetComponent<MeshRenderer>().material.color = Color.grey;
                    selectedToken = null;
                }

                FindObjectOfType<BattleArenaUIManager>().DisableCardPreview();
            }

            if(selectedToken != null)
            {
                if(!(selectedToken as HQManager))
                {
                    selectedToken.actionPuck.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }

        }
    }

    private TokenManager ReturnTokenUnderMouse()
    {
        TokenManager mouseToken = null;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Token"))) { mouseToken = hitInfo.collider.GetComponent<TokenManager>(); }
        return mouseToken;
    }

    private Node ReturnNodeUnderMouse()
    {
        Node mouseNode = null;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Node"))) { mouseNode = hitInfo.collider.GetComponent<Node>(); }
        return mouseNode;
    }

}
