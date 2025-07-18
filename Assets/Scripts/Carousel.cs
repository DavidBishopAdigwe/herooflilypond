using TMPro;
using UnityEngine;

public abstract class Carousel: MonoBehaviour
{
    [SerializeField] protected TMP_Text textElement;
    [SerializeField] protected string currentText;
    protected int OptionIndex;
    protected int OptionsCount;

    protected virtual void Start()
    {
        OnValueChanged();
    }
    public void Next()
    {
        OptionIndex++;
        if (OptionIndex >= OptionsCount)
        {
            OptionIndex = 0;
        }
        OnValueChanged();
    }

    public void Previous()
    {
        OptionIndex--;
        if (OptionIndex < 0)
        {
            OptionIndex = OptionsCount - 1;
        }
        OnValueChanged();

    }

    protected virtual void OnValueChanged()
    {
        textElement.text = currentText;
    }
    
} 

