using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;

    [SerializeField] private Volume postProcessVolume;
    private ColorAdjustments _colorAdj;

    void Awake()
    {
        if (!postProcessVolume.profile.TryGet(out _colorAdj))
            Debug.LogError("Volume Profile has no Color Adjustments override!");

        brightnessSlider.minValue = -2f;
        brightnessSlider.maxValue = 2f;
        brightnessSlider.value    = _colorAdj.postExposure.value;
        brightnessSlider.onValueChanged.AddListener(SetExposure);
    }

    public void SetExposure(float exposure)
    {
        if (_colorAdj != null)
        {
            _colorAdj.postExposure.value = exposure;
        }
    }
}