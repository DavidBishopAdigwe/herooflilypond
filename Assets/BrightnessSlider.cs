using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSlider : MonoBehaviour
{
    private Slider _sliderComponent;

    private void Awake()
    {
        _sliderComponent = gameObject.GetComponent<Slider>();
    }

    public void SliderValueChanged()
    { 
        Screen.brightness = _sliderComponent.value; 
        Debug.Log($"changed from val{Screen.brightness}");
    }
    
}
