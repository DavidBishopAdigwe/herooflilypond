using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{
    [SerializeField] private Vector2[] options;
    [SerializeField] private TMP_Text textElement;
    [SerializeField] private string currentText;
    private int _optionIndex;

    private void Awake()
    {
       SetResolution();
    }

    public void Next()
    {
        _optionIndex++;
        if (_optionIndex >= options.Length)
        {
            _optionIndex = 0;
        } 
        SetResolution();
    }

    public void Previous()
    {
        _optionIndex--;
        if (_optionIndex < 0)
        {
            _optionIndex = options.Length - 1;
        } 
        SetResolution();
    }

    private void SetResolution()
    {
        currentText = $"{options[_optionIndex].x} x {options[_optionIndex].y}";
        textElement.text = currentText;
    }
}
