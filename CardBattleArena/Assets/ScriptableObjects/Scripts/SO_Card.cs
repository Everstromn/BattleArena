using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SO_Card : ScriptableObject
{
    public string cardName;
    public Color32 cardTint;
    public int cost;
    public Sprite cardImage;
    public string cardText;
    public CardType cardType = CardType.Creature;

}
