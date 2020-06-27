using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXSliderController : MonoBehaviour
{
    public GameObject mySider;

    private void Start()
    {
        mySider.GetComponent<Slider>().value = SoundsManager.instance.sfxSource.volume;
    }

    public void OnValueChanged(float _val)
    {
        SoundsManager.instance.sfxSource.volume = _val;
    }
}
