using TMPro;
using UnityEngine;

public class FullscreenCarousel: MonoBehaviour
{
    [SerializeField] private string[] options;
    [SerializeField] private TMP_Text textElement;
    [SerializeField] private string currentText;
    private int _optionIndex;
    private int _optionsLength;

    private void Awake()
    {
        OnValueChanged();
    }

    public void Next()
    {
        _optionIndex++;
        if (_optionIndex >= options.Length)
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
            _optionIndex = options.Length - 1;
        } 
        OnValueChanged();
    }

    private void OnValueChanged()
    {
        textElement.text = currentText;
    }
}