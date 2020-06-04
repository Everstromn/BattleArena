using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQManager : TokenManager
{

    protected override void Start()
    {
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

    public override float ActionPhase(float givenDelay)
    {
        BattleManager.instance.remainingActions--;
        return givenDelay;
    }

    protected override void OnMouseOver()
    {

    }

    protected override void OnMouseExit()
    {

    }

}
