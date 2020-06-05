using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardFadingScript : MonoBehaviour
{
    [SerializeField] private Image cardDisplayImage = null;
    [SerializeField] private TMP_Text cardDisplayText = null;

    private SO_Card myCard;

    public void RetrieveNewCard()
    {
        myCard = FindObjectOfType<MainMenuUIScript>().ReturnRandomCard();

        cardDisplayImage.sprite = myCard.cardImage;
        cardDisplayText.text = myCard.cardName;

    }

}
