
using UnityEngine;
using UnityEngine.UI;

public abstract class SliderObject : MonoBehaviour
{
    [SerializeField] protected float minValue;
    [SerializeField] protected float maxValue;
    
    protected Slider Slider;
    protected virtual void Awake()
    {
        Slider = GetComponent<Slider>();
        Slider.minValue = minValue;
        Slider.maxValue = maxValue;
        Slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    protected abstract void OnSliderValueChanged(float value);
}


