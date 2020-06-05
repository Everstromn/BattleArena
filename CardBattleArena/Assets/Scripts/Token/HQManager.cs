using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQManager : TokenManager
{

    protected override void Start()
    {
        PlayerDeckManager.instance.ShufflePlayerDeck();
        maxHealth = BattleManager.instance.playerStartingHealth;
        currentHealth = maxHealth;
    }

    protected override void Update()
    {
        
    }

    protected override void Death()
    {
        Debug.Log(this.name + " LOSES");
    }

    public override void UpkeepPhase()
    {
        
    }

    public override void ActionPhase(float givenDelay)
    {
        BattleManager.instance.remainingActions--;
    }

    protected override void OnMouseOver()
    {

    }

    protected override void OnMouseExit()
    {

    }
    public override void MoveTokenViaAI(Node givenTargetNode)
    {
        BattleManager.instance.remainingActions--;
    }

}
