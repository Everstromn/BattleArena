using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class BattleArenaUIManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard = null;
    [SerializeField] private GameObject cardDisplayObj = null;
    [SerializeField] private GameObject cardPreviewObj = null;

    [SerializeField] private TMP_Text phaseDisplayTextObj = null;
    [SerializeField] private TMP_Text currentGoldDispalyTextObj = null;
    [SerializeField] private TMP_Text turnGoldDispalyTextObj = null;
    [SerializeField] private TMP_Text turnDisplayTextObj = null;
    [SerializeField] private TMP_Text deckDispalyTextObj = null;

    [SerializeField] private GameObject hoverIcon = null;

    [SerializeField] private GameObject battleEndUI = null;

    [SerializeField] private Button proceedButton = null;
    [SerializeField] private Button redrawButton = null;
    [SerializeField] private TMP_Text redrawButtonCost = null;

    [SerializeField] private CinemachineVirtualCamera cameraPos01 = null;
    [SerializeField] private CinemachineVirtualCamera cameraPos02 = null;
    [SerializeField] private CinemachineVirtualCamera cameraPos03 = null;
    [SerializeField] private CinemachineVirtualCamera cameraPos04 = null;

    [SerializeField] private GameObject arena = null;
    [SerializeField] private Node playerHQNode = null;
    [SerializeField] private Node AIHQNode = null;

    [SerializeField] private GameObject inGameMenu = null;
    [SerializeField] private GameObject inGameGlossary = null;

    private TokenManager selectedToken = null; // rightclick to preview puck - asset

    private int startingDeckSize = 0;
    private int redrawGoldCost = 3;
    private int cameraCurrentPos = 1;

    private void Start()
    {
        UpdateGoldDisplay();
        SetDeckSize();
        StartCoroutine(BattleManager.instance.BattleInitiationDelay());
        BattleManager.instance.Arena = arena;
        BattleManager.instance.playerHQNode = playerHQNode;
        BattleManager.instance.AIHQNode = AIHQNode;
        redrawButtonCost.text = "Redraw Hand \n (" + redrawGoldCost + " Gold)";
    }

    public void RotateCameraPos(int val)
    { 
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

        if(redrawGoldCost <= CurrencyManager.instance.ReturnGold() && BattleManager.instance.currentPhase == BattlePhase.Orders && BattleManager.instance.currentTeamTurn == BattleManager.instance.playerTeam)
        { redrawButton.interactable = true; } else { redrawButton.interactable = false; }

        if (Input.GetKeyDown(KeyCode.Q)) { RotateCameraPos(-1); }
        if (Input.GetKeyDown(KeyCode.E)) { RotateCameraPos(+1); }
        if (Input.GetKeyDown(KeyCode.Escape)) { LoadInGameMenu(); }
        // rightclick to preview puck
        if (BattleManager.instance.battleActive)
        {
            if (Input.GetMouseButtonDown(1))
            {

                if (selectedToken == null && ReturnTokenUnderMouse() != null)
                {
                    if (ReturnTokenUnderMouse().myCard != null)
                    { 
                    DisableCardPreview();
                    EnableCardPreview(ReturnTokenUnderMouse().myCard);
                    }
                }
                
                else
                {
                    FindObjectOfType<BattleArenaUIManager>().DisableCardPreview();
                }
            }
        }
    }

    public void UpdateGoldDisplay()
    {
        int turnGold = CurrencyManager.instance.goldPerTurnAddition + CurrencyManager.instance.goldPerTurnBase;
        string sign;
        if(turnGold > 0) { sign = "+"; } else { sign = "-"; }
        currentGoldDispalyTextObj.text = CurrencyManager.instance.ReturnGold().ToString();
        turnGoldDispalyTextObj.text = "( " + sign + turnGold.ToString() + " )";
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

        deckDispalyTextObj.text = PlayerDeckManager.instance.ReturnDeckSize().ToString() + " / " + startingDeckSize; 
    }

    public void DisplayHover(TokenManager token) { hoverIcon.SetActive(true); hoverIcon.GetComponent<TokenHoverManager>().UpdateDisplay(token); }
    public void HideHover() { hoverIcon.SetActive(false); }
    public void SetDeckSize() { startingDeckSize = PlayerDeckManager.instance.ReturnDeckSize(); }

    public void EnableCardPreview(SO_Card givenCard)
    {
        cardPreviewObj.SetActive(true);
        cardPreviewObj.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(cardPreviewObj, new Vector3(1.5f, 1.5f, 1.5f), 0.25f);

        cardPreviewObj.GetComponent<CardManager>().myCard = givenCard;
        cardPreviewObj.GetComponent<CardManager>().UpdateValuesFromCardAsset();
    }
    public void DisableCardPreview() { cardPreviewObj.SetActive(false); }

    public void EnableBattleEnd()
    {
        battleEndUI.SetActive(true);
    }

    public void RedrawHand()
    {
        CurrencyManager.instance.AlterGold(-redrawGoldCost);
        redrawGoldCost++;
        redrawButtonCost.text = "Redraw Hand \n(" + redrawGoldCost + " Gold)";

        int cardsToDraw = PlayerDeckManager.instance.playerHand.Count;
        
        foreach (SO_Card handCard in PlayerDeckManager.instance.playerHand)
        {
            PlayerDeckManager.instance.AddCardToDeck(handCard);
        }

        PlayerDeckManager.instance.playerHand.Clear();
        PlayerDeckManager.instance.ShufflePlayerDeck();

        for (int i = 0; i < cardsToDraw; i++)
        {
            PlayerDeckManager.instance.DrawCardFromDeck();
        }

        UpdateGoldDisplay();
        UpdateHandDisplay();
    }

    public void LoadInGameMenu() { inGameMenu.SetActive(true); }
    public void LoadInGameGlossary() { inGameGlossary.SetActive(true); inGameGlossary.GetComponent<Glossary>().OnLoad(); }

    private TokenManager ReturnTokenUnderMouse() // rightclick to preview puck - asset
    {
        TokenManager mouseToken = null;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Token"))) { mouseToken = hitInfo.collider.GetComponent<TokenManager>(); }
        return mouseToken;
    }
}
