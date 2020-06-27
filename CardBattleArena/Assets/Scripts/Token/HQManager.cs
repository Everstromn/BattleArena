using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQManager : TokenManager
{
    private int playerHealth;
    protected override void Start()
    {
        PlayerDeckManager.instance.ShufflePlayerDeck();
        maxHealth = BattleManager.instance.playerStartingHealth;
        currentHealth = maxHealth;
        RecalcBuffs();
    }

    protected override void Update()
    {
        
    }

    public override void Death()
    {
        HQManager[] players = FindObjectsOfType<HQManager>();

        foreach (HQManager hQManager in players)
        {
            if(hQManager.myTeam == BattleManager.instance.playerTeam) { playerHealth = hQManager.currentHealth; break; }
        }

        if (myTeam == BattleManager.instance.playerTeam)
        {
            FindObjectOfType<BattleArenaUIManager>().EnableBattleEnd();
            FindObjectOfType<BattleEndUI>().UpdateDisplay("DEFEAT", BattleManager.instance.turnCounter, playerHealth);
            SoundsManager.instance.PlayDefeatSound();
        }
        else
        {
            FindObjectOfType<BattleArenaUIManager>().EnableBattleEnd();
            FindObjectOfType<BattleEndUI>().UpdateDisplay("VICTORY", BattleManager.instance.turnCounter, playerHealth);
            SoundsManager.instance.PlayVictorySound();
        }
    }

    public override void UpkeepPhase()
    {
        
    }

    public override void ActionPhase(float givenDelay)
    {
        BattleManager.instance.remainingActions--;
    }

    public override void MoveTokenViaAI(Node givenTargetNode)
    {
        BattleManager.instance.remainingActions--;
    }

}
