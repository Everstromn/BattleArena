using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseTrackingButton : MonoBehaviour
{
    private Image myImage;

    private Color myBaseColor = Color.gray;
    private Color myActiveColor = Color.blue;

    public BattlePhase myPhase;

    private void Start()
    {
        myImage = GetComponent<Image>();
    }

    private void Update()
    {
        if(BattleManager.instance.currentPhase == myPhase)
        {
            if (BattleManager.instance.currentTeamTurn == BattleManager.instance.playerTeam)
            { myImage.color = BattleManager.instance.playerTeamColor; } else { myImage.color = BattleManager.instance.otherTeamColor; }
        }
        else
        {
            myImage.color = myBaseColor;
        }
    }

}
