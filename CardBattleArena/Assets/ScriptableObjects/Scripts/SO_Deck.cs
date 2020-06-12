using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Deck", menuName = "Cards/Deck")]
public class SO_Deck : ScriptableObject
{
    public string deckName;
    public SO_Card[] deck = new SO_Card[50]; 
}
