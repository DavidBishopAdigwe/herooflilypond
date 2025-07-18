using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BrightnessSlider : SliderObject
{
    [SerializeField] private Image brightnessOverlay;
    

    protected override void Awake()
    {
        base.Awake();
        Slider.value = 1 - brightnessOverlay.color.a;
    }

    protected override void OnSliderValueChanged(float value)
    {
        var imageColor = brightnessOverlay.color;
        imageColor.a = 1 - value;
        brightnessOverlay.color = imageColor;
    }
    
}
