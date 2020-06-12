using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TokenHoverManager : MonoBehaviour
{

    [Space(20)]
    [Header("Creature Card Hover References")]
    [SerializeField] private GameObject     creatureHoverObj = null;
    [SerializeField] private Image          c_hoverImage= null;
    [SerializeField] private TMP_Text       c_hoverTitle= null;
    [SerializeField] private TMP_Text       c_hoverDesc= null;
    [SerializeField] private TMP_Text       c_hoverRangeText= null;
    [SerializeField] private TMP_Text       c_hoverMovementText= null;
    [SerializeField] private TMP_Text       c_hoverDamageText= null;
    [SerializeField] private TMP_Text       c_hoverMaxHealthText= null;
    [SerializeField] private TMP_Text       c_hoverCurrentHealthText= null;
    [SerializeField] private RectTransform  c_hoverHealthForegroundObj = null;

    [Space(20)]
    [Header("Creature Card Hover References")]
    [SerializeField] private GameObject buildingHoverObj = null;
    [SerializeField] private Image b_hoverImage = null;
    [SerializeField] private TMP_Text b_hoverTitle = null;
    [SerializeField] private TMP_Text b_hoverDesc = null;
    [SerializeField] private TMP_Text b_hoverCurrentHealthText = null;
    [SerializeField] private RectTransform b_hoverHealthForegroundObj = null;

    [Space(20)]
    [Header("HQ Hover References")]
    [SerializeField] private GameObject HQHoverObj = null;
    [SerializeField] private Image hq_hoverImage = null;
    [SerializeField] private TMP_Text hq_hoverTitle = null;
    [SerializeField] private TMP_Text hq_hoverCurrentHealthText = null;
    [SerializeField] private RectTransform hq_hoverHealthForegroundObj = null;

    private float width;

    private void Start()
    {
        width = c_hoverHealthForegroundObj.rect.width;
    }

    public void UpdateDisplay(TokenManager token)
    {
        if (token as HQManager)
        {
            HQHoverObj.SetActive(true);
            buildingHoverObj.SetActive(false);
            creatureHoverObj.SetActive(false);

            //hq_hoverImage.sprite = token.myCard.cardImage;
            hq_hoverTitle.text = token.myTeam.ToString() + " Teams HQ";

            float healthpct = (float)token.currentHealth / token.maxHealth;

            hq_hoverCurrentHealthText.text = token.currentHealth.ToString() + " / " + token.maxHealth.ToString();
            hq_hoverHealthForegroundObj.sizeDelta = new Vector2((healthpct * 216), hq_hoverHealthForegroundObj.sizeDelta.y);
        }

        else if (token.myCard.cardType == CardType.Creature)
        {
            creatureHoverObj.SetActive(true);
            buildingHoverObj.SetActive(false);
            HQHoverObj.SetActive(false);

            c_hoverImage.sprite = token.myCard.cardImage;
            c_hoverTitle.text = token.myCard.cardName;
            c_hoverDesc.text = token.myCard.cardText;
            
            c_hoverRangeText.text = token.myCreatureCard.attackRange.ToString();
            c_hoverMovementText.text = token.myCreatureCard.movement.ToString();
            c_hoverDamageText.text = token.myCreatureCard.damage.ToString();
            c_hoverMaxHealthText.text = token.myCreatureCard.health.ToString();

            float healthpct = (float)token.currentHealth / token.myCreatureCard.health;

            c_hoverCurrentHealthText.text = token.currentHealth.ToString() + " / " + token.myCreatureCard.health.ToString();
            c_hoverHealthForegroundObj.sizeDelta = new Vector2((healthpct * 216), c_hoverHealthForegroundObj.sizeDelta.y);
        }

        else if (token.myCard.cardType == CardType.Building)
        {
            buildingHoverObj.SetActive(true);
            creatureHoverObj.SetActive(false);
            HQHoverObj.SetActive(false);

            b_hoverImage.sprite = token.myCard.cardImage;
            b_hoverTitle.text = token.myCard.cardName;
            b_hoverDesc.text = token.myCard.cardText;

            float healthpct = (float)token.currentHealth / token.myBuildingCard.health;

            b_hoverCurrentHealthText.text = token.currentHealth.ToString() + " / " + token.myBuildingCard.health.ToString();
            b_hoverHealthForegroundObj.sizeDelta = new Vector2((healthpct * 216), b_hoverHealthForegroundObj.sizeDelta.y);
        }
    }



}
