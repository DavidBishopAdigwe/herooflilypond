using System;
using System.Collections.Generic;
using DataPersistence.Data;
using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FullscreenCarousel: MonoBehaviour, IDataPersistence
{
    [SerializeField] private TMP_Text textElement;
    [SerializeField] private string currentText;
    [SerializeField] private UnityEvent<bool> onValChanged;
    [SerializeField] private List<string> options;
    private int _optionIndex;
    private bool _fullScreen;


    private void Start()
    {
        OnValueChanged();
    }

    public void Next()
    {
        _optionIndex++;
        if (_optionIndex >= options.Count)
        {
            _optionIndex = 0;
        }
        OnValueChanged();
    }

    public void Previous()
    {
        _optionIndex--;
        if (_optionIndex < 0)
        {
            _optionIndex = options.Count - 1;
        }
        OnValueChanged();

    }

    private  void OnValueChanged()
    {
        currentText = options[_optionIndex];
        textElement.text = currentText;
        _fullScreen = _optionIndex == 1;
        ScreenControl.Instance.SetFullScreen(_fullScreen);
    }

    public void LoadData(GameData data)
    {
    }

    public void SaveData(ref GameData data)
    {
    }
}