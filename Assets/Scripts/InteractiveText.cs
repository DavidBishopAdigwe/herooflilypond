using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InteractiveText : MonoBehaviour
{
    [SerializeField] private Color selectColor;
    [SerializeField] private Color deselectColor;
    private TMP_Text _textElement;

    private void Awake()
    {
        _textElement = GetComponentInChildren<TMP_Text>();
    }

    public void SelectText()
    {
        _textElement.color = selectColor;
    }

    public void DeselectText()
    {
        _textElement.color = deselectColor;
    }
}
