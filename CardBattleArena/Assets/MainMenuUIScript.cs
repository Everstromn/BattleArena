using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuUIScript : MonoBehaviour
{

    [SerializeField] private SO_Card[] cardFadingCards = null;
    [SerializeField] private GameObject sessionManager = null;

    private void Start()
    {
        if(!FindObjectOfType<BattleManager>())
        {
            Instantiate(sessionManager);
        }
    }

    public SO_Card ReturnRandomCard()
    {
        return cardFadingCards[Random.Range(0, cardFadingCards.Length)];
    }

    public void LoadMainBattle()
    {
        SoundsManager.instance.PlayClickSound();
        SceneManager.LoadScene(1);
    }

}
