using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSliderController : MonoBehaviour
{
    public GameObject mySider;

    private void Start()
    {
        mySider.GetComponent<Slider>().value = SoundsManager.instance.musicSource.volume;
    }

    public void OnValueChanged(float _val)
    {
        SoundsManager.instance.musicSource.volume = _val;
    }
}
