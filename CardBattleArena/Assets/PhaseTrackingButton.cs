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
        if(BattleManager.instance.currentPhase == myPhase) { myImage.color = myActiveColor; } else { myImage.color = myBaseColor; }
    }

}
