using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using DataPersistence;
using DataPersistence.Data;
using Interfaces;
using UnityEngine.Serialization;

public class Resolution : MonoBehaviour, IDataPersistence
{
    [SerializeField] private TMP_Text textElement;
    [SerializeField] private string currentText;
    [SerializeField] private UnityEvent<int, int> onValChanged;
    [SerializeField] private List<Vector2Int> options;
    private int _optionIndex;
    private Vector2Int _currentResolution = new Vector2Int();

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
        _currentResolution = new Vector2Int(options[_optionIndex].x, options[_optionIndex].y);
        currentText = $"{_currentResolution.x} x {_currentResolution.y}";
        onValChanged.Invoke(_currentResolution.x,_currentResolution.y);
        textElement.text = currentText;
    }

    public void LoadData(GameData data)
    {
        Debug.Log(data.res[0]);
        var currentRes = new Vector2Int(data.res[0], data.res[1]);
        if (options.Contains(currentRes))
        {
            int resIndex = options.IndexOf(currentRes);
            _optionIndex = resIndex;
            OnValueChanged();
        }
    }

    public void SaveData(ref GameData data)
    {
        data.res[0] = _currentResolution.x;
        data.res[1] = _currentResolution.y;
    }
}
