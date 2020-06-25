using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject concedeGamePanel = null;
    [SerializeField] private CanvasGroup myCanvasGroup = null;

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void ConcedeGame()
    {
        concedeGamePanel.SetActive(true);
        myCanvasGroup.interactable = false;
    }

    public void FightOn()
    {
        concedeGamePanel.SetActive(false);
        myCanvasGroup.interactable = true;
    }

    public void LoadGlossary()
    {
        FindObjectOfType<BattleArenaUIManager>().LoadInGameGlossary();
    }

    public void ConfirmConcedeGame()
    {

        HQManager[] players = FindObjectsOfType<HQManager>();

        foreach (HQManager hQManager in players)
        {
            if (hQManager.myTeam == BattleManager.instance.playerTeam) { hQManager.Death(); }
        }

        myCanvasGroup.interactable = false;
        CloseMenu();
    }


}
