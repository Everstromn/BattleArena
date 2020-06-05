﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class BattleArenaUIManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard = null;
    [SerializeField] private GameObject cardDisplayObj = null;
    [SerializeField] private TMP_Text phaseDisplayTextObj = null;
    [SerializeField] private TMP_Text currentGoldDispalyTextObj = null;
    [SerializeField] private TMP_Text turnGoldDispalyTextObj = null;
    [SerializeField] private TMP_Text turnDisplayTextObj = null;

    [SerializeField] private GameObject hoverIcon = null;

    [SerializeField] private Button proceedButton = null;

    [SerializeField] private CinemachineVirtualCamera cameraPos01 = null;
    [SerializeField] private CinemachineVirtualCamera cameraPos02 = null;
    [SerializeField] private CinemachineVirtualCamera cameraPos03 = null;
    [SerializeField] private CinemachineVirtualCamera cameraPos04 = null;

    private int cameraCurrentPos = 1;

    public void RotateCameraPos(int val)
    {
        Debug.Log("Called");

        if (cameraCurrentPos == 4 && val == 1) { cameraCurrentPos = 1; }
        else if (cameraCurrentPos == 1 && val == -1) { cameraCurrentPos = 4; }
        else cameraCurrentPos = cameraCurrentPos + val;

       if(cameraCurrentPos == 1)
        {
                cameraPos01.Priority = 20;
                cameraPos02.Priority = 10;
                cameraPos03.Priority = 10;
                cameraPos04.Priority = 10;
        }

        if (cameraCurrentPos == 2)
        {
                cameraPos01.Priority = 10;
                cameraPos02.Priority = 20;
                cameraPos03.Priority = 10;
                cameraPos04.Priority = 10;
        }

        if (cameraCurrentPos == 3)
        {
                cameraPos01.Priority = 10;
                cameraPos02.Priority = 10;
                cameraPos03.Priority = 20;
                cameraPos04.Priority = 10;
        }

        if (cameraCurrentPos == 4)
        {
                cameraPos01.Priority = 10;
                cameraPos02.Priority = 10;
                cameraPos03.Priority = 10;
                cameraPos04.Priority = 20;
        }

    }

    public void TriggerNextPhase()
    {
        BattleManager.instance.remainingActions = 0;
    }

    private void Update()
    {
        phaseDisplayTextObj.text = BattleManager.instance.currentPhase.ToString();
        turnDisplayTextObj.text = "Turn : " + BattleManager.instance.turnCounter.ToString();

        if (BattleManager.instance.currentPhase == BattlePhase.Orders && BattleManager.instance.currentTeamTurn == BattleManager.instance.playerTeam)
        { proceedButton.interactable = true; } else { proceedButton.interactable = false; }

        if (Input.GetKeyDown(KeyCode.Q)) { RotateCameraPos(-1); }
        if (Input.GetKeyDown(KeyCode.E)) { RotateCameraPos(+1); }
    }

    public void UpdateGoldDisplay()
    {
        currentGoldDispalyTextObj.text = CurrencyManager.instance.ReturnGold().ToString();
        turnGoldDispalyTextObj.text = (CurrencyManager.instance.goldPerTurnAddition + CurrencyManager.instance.goldPerTurnBase).ToString();
    }

    public void UpdateHandDisplay()
    {

        foreach (Transform card in cardDisplayObj.transform)
        {
            Destroy(card.gameObject);
        }

        foreach (SO_Card card in PlayerDeckManager.instance.playerHand)
        {
            GameObject newCard = Instantiate(prefabCard, cardDisplayObj.transform);
            newCard.GetComponent<CardManager>().myCard = card;
            newCard.GetComponent<CardManager>().UpdateValuesFromCardAsset();
        }
    }

    public void DisplayHover(TokenManager token) { hoverIcon.SetActive(true); hoverIcon.GetComponent<TokenHoverManager>().UpdateDisplay(token); }
    public void HideHover() { hoverIcon.SetActive(false); }

}