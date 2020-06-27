using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject concedeGamePanel = null;
    [SerializeField] private CanvasGroup myCanvasGroup = null;
    [SerializeField] private GameObject optionsPanel = null;

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        SoundsManager.instance.PlayClickSound();
    }

    public void ConcedeGame()
    {
        concedeGamePanel.SetActive(true);
        myCanvasGroup.interactable = false;
        SoundsManager.instance.PlayClickSound();
    }

    public void FightOn()
    {
        concedeGamePanel.SetActive(false);
        myCanvasGroup.interactable = true;
        SoundsManager.instance.PlayClickSound();
    }

    public void LoadGlossary() { FindObjectOfType<BattleArenaUIManager>().LoadInGameGlossary(); }
    public void LoadOptions() { optionsPanel.SetActive(true); myCanvasGroup.interactable = false; SoundsManager.instance.PlayClickSound(); }

    public void CloseOptions() { optionsPanel.SetActive(false); myCanvasGroup.interactable = true; SoundsManager.instance.PlayClickSound(); }

    public void ConfirmConcedeGame()
    {

        HQManager[] players = FindObjectsOfType<HQManager>();

        foreach (HQManager hQManager in players)
        {
            if (hQManager.myTeam == BattleManager.instance.playerTeam) { hQManager.Death(); }
        }

        myCanvasGroup.interactable = false;
        SoundsManager.instance.PlayClickSound();
        CloseMenu();
    }


}
