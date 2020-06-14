using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour, IPointerClickHandler
{
    [Header("Random References")]
    public bool preview = false;
    [SerializeField] private Button cardButtonObj = null;

    [Space(20)]
    [Header("Card Glow Reference")]
    [SerializeField] private Image creatureGlowImageObj = null;
    [SerializeField] private Color playableGlowColor = Color.yellow;

    [Space(20)]
    [Header("Creature Card Object References")]
    [SerializeField] private GameObject creatureCardFront = null;
    [SerializeField] private TMP_Text c_cardNameTextObj = null;
    [SerializeField] private Image c_cardTitleTintImageObj = null;
    [SerializeField] private TMP_Text c_cardCostTextObj = null;
    [SerializeField] private Image c_cardImageObj = null;
    [SerializeField] private TMP_Text c_cardDescTextObj = null;
    [SerializeField] private Image c_cardDescTintImageObj = null;
    [SerializeField] private TMP_Text c_cardAttackRangeTextObj= null;
    [SerializeField] private TMP_Text c_cardMovementTextObj= null;
    [SerializeField] private TMP_Text c_cardDamageTextObj = null;
    [SerializeField] private TMP_Text c_cardHealthTextObj = null;

    [Space(20)]
    [Header("Building Card Object References")]
    [SerializeField] private GameObject buildingCardFront = null;
    [SerializeField] private TMP_Text b_cardNameTextObj = null;
    [SerializeField] private Image b_cardTitleTintImageObj = null;
    [SerializeField] private TMP_Text b_cardCostTextObj = null;
    [SerializeField] private Image b_cardImageObj = null;
    [SerializeField] private TMP_Text b_cardDescTextObj = null;
    [SerializeField] private Image b_cardDescTintImageObj = null;

    [HideInInspector] public SO_Card myCard;
    private SO_Creature myCreatureCard;
    private SO_Building myBuildingCard;

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
                buildingCardFront.SetActive(false);
                creatureCardFront.SetActive(true);

                c_cardNameTextObj.text = myCard.cardName;
                c_cardTitleTintImageObj.color = myCard.cardTint;
                c_cardCostTextObj.text = myCard.cost.ToString();
                c_cardImageObj.sprite = myCard.cardImage;
                c_cardDescTextObj.text = myCard.cardText;
                c_cardDescTintImageObj.color = myCard.cardTint;

                c_cardAttackRangeTextObj.text = myCreatureCard.attackRange.ToString();
                c_cardMovementTextObj.text = myCreatureCard.movement.ToString();
                c_cardDamageTextObj.text = myCreatureCard.damage.ToString();
                c_cardHealthTextObj.text = myCreatureCard.health.ToString();
            }

            if(myCard.cardType == CardType.Building)
            {
                myBuildingCard = myCard as SO_Building;
                buildingCardFront.SetActive(true);
                creatureCardFront.SetActive(false);


                b_cardNameTextObj.text = myCard.cardName;
                b_cardTitleTintImageObj.color = myCard.cardTint;
                b_cardCostTextObj.text = myCard.cost.ToString();
                b_cardImageObj.sprite = myCard.cardImage;
                b_cardDescTextObj.text = myCard.cardText;
                b_cardDescTintImageObj.color = myCard.cardTint;

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
        if (!preview)
        {
        BattleManager.instance.SpawnTokenFromCard(myCard);
        PlayerDeckManager.instance.playerHand.Remove(myCard);
        Destroy(gameObject);
        }
        else
        {
            FindObjectOfType<BattleArenaUIManager>().DisableCardPreview();
        }

    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(!preview)
            {
                FindObjectOfType<BattleArenaUIManager>().EnableCardPreview(myCard);
            }
            else
            {
                FindObjectOfType<BattleArenaUIManager>().DisableCardPreview();
            }
        }
    }
}
