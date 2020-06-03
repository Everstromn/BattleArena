using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardManager : MonoBehaviour
{
    [Header("Card Glow Reference")]
    [SerializeField] private Image creatureGlowImageObj = null;
    [SerializeField] private Color playableGlowColor = Color.yellow;

    [Header("Creature Card Object References")]
    [SerializeField] private GameObject creatureCardFront = null;
    [SerializeField] private TMP_Text cardNameTextObj = null;
    [SerializeField] private Image cardTitleTintImageObj = null;
    [SerializeField] private TMP_Text cardCostTextObj = null;
    [SerializeField] private Image cardImageObj = null;
    [SerializeField] private TMP_Text cardDescTextObj = null;
    [SerializeField] private Image cardDescTintImageObj = null;
    [SerializeField] private Image cardAttackPatternImageObj= null;
    [SerializeField] private TMP_Text cardMovementTextObj= null;
    [SerializeField] private TMP_Text cardDamageTextObj = null;
    [SerializeField] private TMP_Text cardHealthTextObj = null;
    [SerializeField] private Button cardButtonObj = null;

    public SO_Card myCard;
    private SO_Creature myCreatureCard;

    private bool canBePlayedNow = false;
    public bool CanBePlayedNow
    {
        get { return canBePlayedNow; }
        set { canBePlayedNow = value;  }
    }

    private void Awake()
    {
        if(myCard != null) { UpdateValuesFromCardAsset(); }
    }

    private void Update()
    {
        if(myCard.cost <= CurrencyManager.instance.ReturnGold()) { cardButtonObj.interactable = true; ActivateCardGlow(playableGlowColor); } else { cardButtonObj.interactable = false; DeActivateCardGlow(); }
    }

    public void UpdateValuesFromCardAsset()
    {
        if(myCard != null)
        {
            if(myCard.cardType == CardType.Creature)
            {
                myCreatureCard = myCard as SO_Creature;
                creatureCardFront.SetActive(true);

                cardNameTextObj.text = myCard.cardName;
                cardTitleTintImageObj.color = myCard.cardTint;
                cardCostTextObj.text = myCard.cost.ToString();
                cardImageObj.sprite = myCard.cardImage;
                cardDescTextObj.text = myCard.cardText;
                cardDescTintImageObj.color = myCard.cardTint;

                cardAttackPatternImageObj.sprite = myCreatureCard.attackPattern;
                cardMovementTextObj.text = myCreatureCard.movement.ToString();
                cardDamageTextObj.text = myCreatureCard.damage.ToString();
                cardHealthTextObj.text = myCreatureCard.health.ToString();
            }
        }


    }

    public void ActivateCardGlow(Color glowColor)
    {
        creatureGlowImageObj.gameObject.SetActive(true);
        creatureGlowImageObj.color = glowColor;
    }

    public void DeActivateCardGlow()
    {
        creatureGlowImageObj.gameObject.SetActive(false);
    }

    public void OnCardClick()
    {
        BattleManager.instance.SpawnTokenFromCard(myCard);
        PlayerDeckManager.instance.playerHand.Remove(myCard);
        Destroy(gameObject);
    }

}
