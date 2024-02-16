using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public Slider _sfxSlider, _accessibleSlider, _UISlider, _narratorSlider, _gameSlider;
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
    public void SFXVolume()
    {
        AudioManager.instance.SFXVolume(_sfxSlider.value);
    }

    public void UIVolume()
    {
        AudioManager.instance.UIVolume(_UISlider.value);
    }

    public void DescriptionVolume()
    {
        AudioManager.instance.DescriptionVolume(_narratorSlider.value);
    }

    public void AccessibleDescriptionVolume()
    {
        AudioManager.instance.AccessibleDescriptionVolume(_accessibleSlider.value);
    }

    public void GameVolume()
    {
        AudioManager.instance.GameVolume(_gameSlider.value);
    }


}
