using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeckManager : MonoBehaviour
{
    public static PlayerDeckManager instance;
    private void Awake()
    {

        int gameManagerCount = FindObjectsOfType<PlayerDeckManager>().Length;

        if (gameManagerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        if (instance != null)
        {
            Debug.LogError("More than one Player Deck Manager");
        }
        instance = this;
    }


    [SerializeField] private List<SO_Card> playerDeck = new List<SO_Card>();
    public List<SO_Card> playerHand = new List<SO_Card>();

    public void AddCardToDeck(SO_Card cardToAdd) { playerDeck.Add(cardToAdd); }

    public void DrawCardFromDeck()
    {
        if(playerDeck.Count > 0)
        {
            SO_Card drawnCard;
            drawnCard = playerDeck[0];
            playerDeck.RemoveAt(0);
            playerHand.Add(drawnCard);

        }
        else
        {
            Debug.Log("No Cards In Deck - UH OH!!!");
        }
    }

    public void ShufflePlayerDeck()
    {
        Debug.Log("Shuffling Deck");

        for (int i = 0; i < playerDeck.Count; i++)
        {
            SO_Card temp = playerDeck[i];
            int randomIndex = Random.Range(i, playerDeck.Count);
            playerDeck[i] = playerDeck[randomIndex];
            playerDeck[randomIndex] = temp;
        }
    }

}
