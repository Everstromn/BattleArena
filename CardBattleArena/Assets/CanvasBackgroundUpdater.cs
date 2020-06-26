using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBackgroundUpdater : MonoBehaviour
{

    [SerializeField] Image myBackgroundImage = null;

    private void Start()
    {
        myBackgroundImage.sprite = PlayerDeckManager.instance.assignedDeck.background;
    }

}
