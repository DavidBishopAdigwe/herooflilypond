using System.Collections.Generic;
using Singletons;
using UnityEngine;
using UnityEngine.Events;

public class FullscreenCarousel: Carousel
{

    [SerializeField] private List<string> options;
    private bool _fullScreen;


    protected override void Start()
    {
        OptionsCount = options.Count;
        OnValueChanged();
    }


    protected override void OnValueChanged()
    {
        currentText = options[OptionIndex];
        _fullScreen = OptionIndex == 1;
        ScreenController.Instance.SetFullScreen(_fullScreen);
        base.OnValueChanged();
    }
    
}