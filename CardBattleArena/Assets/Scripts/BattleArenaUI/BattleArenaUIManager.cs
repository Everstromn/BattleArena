using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleArenaUIManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard = null;
    [SerializeField] private GameObject cardDisplayObj = null;
    [SerializeField] private TMP_Text phaseDisplayTextObj = null;
    [SerializeField] private TMP_Text currentGoldDispalyTextObj = null;
    [SerializeField] private TMP_Text turnGoldDispalyTextObj = null;
    [SerializeField] private TMP_Text turnDisplayTextObj = null;

    [SerializeField] private GameObject hoverIcon = null;

    public void TriggerNextPhase()
    {
        BattleManager.instance.remainingActions = 0;
    }

    private void Update()
    {
        phaseDisplayTextObj.text = BattleManager.instance.currentPhase.ToString();
        turnDisplayTextObj.text = "Turn : " + BattleManager.instance.turnCounter.ToString();
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
