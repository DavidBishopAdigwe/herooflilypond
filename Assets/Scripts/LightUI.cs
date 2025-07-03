
using System;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class LightUI : MonoBehaviour
{ 
    [SerializeField] private RectTransform imageComponent;

    private float _lightDuration;
    private float _originalHeight;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalHeight = _rectTransform.sizeDelta.y;
        
    }
    
    public void GetUIDuration(float lightDuration)
    {
        this._lightDuration = lightDuration;
    }

    public void ChangeSize(float timeRemaining)
    {
        var size = imageComponent.sizeDelta;
        float timeNormalized = Mathf.Clamp01(timeRemaining / _lightDuration );
        size.y = timeNormalized * _originalHeight;
        imageComponent.sizeDelta = size;
    }
}
