using Singletons;
using UnityEngine;




public class VolumeSlider : SliderObject
{
    [SerializeField] private string parameterName;
    private const float MinDb = -80f;
    private const float MaxDb = 0f;

    protected override void Awake()
    {
        base.Awake();
        minValue = MinDb;
        maxValue = MaxDb;
        Slider.value = VolumeManager.Instance.GetVolume(parameterName);
    }

    protected override void OnSliderValueChanged(float value)
    {
        VolumeManager.Instance.SetVolume(parameterName, value);
    }
}



