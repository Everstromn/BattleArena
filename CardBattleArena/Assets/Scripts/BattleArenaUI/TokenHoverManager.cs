using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TokenHoverManager : MonoBehaviour
{

    [SerializeField] private Image hoverImage= null;
    [SerializeField] private TMP_Text hoverTitle= null;
    [SerializeField] private TMP_Text hoverDesc= null;
    [SerializeField] private TMP_Text hoverRangeText= null;
    [SerializeField] private TMP_Text hoverMovementText= null;
    [SerializeField] private TMP_Text hoverDamageText= null;
    [SerializeField] private TMP_Text hoverMaxHealthText= null;
    [SerializeField] private TMP_Text hoverCurrentHealthText= null;

    [SerializeField] private RectTransform hoverHealthForegroundObj = null;

    public void UpdateDisplay(TokenManager token)
    {
        hoverImage.sprite = token.myCard.cardImage;
        hoverTitle.text = token.myCard.cardName;
        hoverDesc.text = token.myCard.cardText;

        hoverRangeText.text = token.myCreatureCard.attackRange.ToString();
        hoverMovementText.text = token.myCreatureCard.movement.ToString();
        hoverDamageText.text = token.myCreatureCard.damage.ToString();
        hoverMaxHealthText.text = token.myCreatureCard.health.ToString();

        float healthpct = (float)token.currentHealth / token.myCreatureCard.health;

        hoverCurrentHealthText.text = token.currentHealth.ToString() + " / " + token.myCreatureCard.health.ToString();
        hoverHealthForegroundObj.sizeDelta = new Vector2((healthpct * 144), hoverHealthForegroundObj.sizeDelta.y);
    }



}
