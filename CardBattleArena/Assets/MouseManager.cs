using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private TokenManager selectedToken;


    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (selectedToken != null && ReturnTokenUnderMouse() == null)
            {
                if(ReturnNodeUnderMouse() != null)
                {
                    selectedToken.MoveToken();
                    selectedToken = null;
                }
                else
                {
                    selectedToken.movementPreview = false;
                    selectedToken.ClearPathHighlights();
                    selectedToken = null;
                    Debug.Log("Deselected Token");
                }

            }

            if (selectedToken == null && ReturnTokenUnderMouse() != null) { selectedToken = ReturnTokenUnderMouse(); selectedToken.movementPreview = true; Debug.Log("Selected Token" + selectedToken); }
            }

        else if(Input.GetMouseButtonDown(1))
        {
            if (selectedToken != null)
            {
                selectedToken.movementPreview = false;
                selectedToken.ClearPathHighlights();
                selectedToken = null;
                Debug.Log("Deselected Token");
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
