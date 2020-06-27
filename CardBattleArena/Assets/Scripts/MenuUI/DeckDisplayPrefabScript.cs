using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckDisplayPrefabScript : MonoBehaviour
{
    [SerializeField] private TMP_Text deckName = null;

    private SO_Deck myDeck;


    public void OnSpawn(SO_Deck _deck)
    {
        myDeck = _deck;
        deckName.text = myDeck.deckName;
    }

    public void SelectDeck()
    {
        PlayerDeckManager.instance.assignedDeck = myDeck;
        PlayerDeckManager.instance.RefreshDeck();
        FindObjectOfType<BattleSelectScreenUI>().UpdateDeckDisplay(myDeck);
        SoundsManager.instance.PlayClickSound();
    }
}
