using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Singletons;


public class ResolutionCarousel : Carousel
{
    [SerializeField] private List<Vector2Int> options;
    private Vector2Int _currentResolution = new Vector2Int();

    protected override void Start()
    {
        OptionsCount = options.Count;
        base.Start();
    }

    protected override void OnValueChanged()
    {
        _currentResolution = new Vector2Int(options[OptionIndex].x, options[OptionIndex].y);
        currentText = $"{_currentResolution.x} x {_currentResolution.y}";
        ScreenController.Instance.SetResolution(_currentResolution.x, _currentResolution.y);
        base.OnValueChanged();
    }
    
}
