using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SO_Card : ScriptableObject
{
    [Space(20)]
    [Header("Generic Card Data")]
    public string cardName;
    public Color32 cardTint;
    public int cost;
    public Sprite cardImage;
    [TextArea(5,10)] public string cardText;
    [TextArea(5, 10)] public string cardFlavorText;
    public CardType cardType = CardType.Creature;

}
