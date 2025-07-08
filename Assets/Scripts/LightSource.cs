using System;
using System.Collections;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class LightSource : MonoBehaviour
{

    [Header("Base Settings")] 
    [SerializeField] private float baseDuration;
    [SerializeField] private float baseIntensity;
    [SerializeField] private float baseRadius;
    [SerializeField] private float baseInnerRadius;

    [Header("Minimum Settings")]
    [SerializeField] private float minIntensity;
    [SerializeField] private float minRadius;
    [SerializeField] private float minInnerRadius;
    [SerializeField] private int secondsToRefill;
    
    
    [SerializeField] UnityEvent refillingLamp;

    private CircleCollider2D _lightCollider;
    private Light2D _light;
    private float _timeRemaining;
    public event Action<bool> OnLightToggled;
    private Coroutine _currentRoutine;
    private bool _lightOn;
    private PlayerItemTracker _playerItemTracker;


    private void Awake()
    {
        _lightCollider = GetComponent<CircleCollider2D>();
        _light = GetComponent<Light2D>();
        _playerItemTracker = GetComponentInParent<PlayerItemTracker>();

    }

    private void Start()
    {
        ResetLight();
    }

    public void ToggleLight()
    {
        if (!_playerItemTracker.PlayerHasLamp()) return;
        if (!_lightOn)
        {
            EnableLight();
            
        }
        else
        {
            DisableLight();
        }
    }
    

    private IEnumerator DecrementLight()
    {
        var intensity               = (baseIntensity - minIntensity) / baseDuration;
        var radius                  = (baseRadius -minRadius) / baseDuration; // Collider
        var innerRadius             = (baseInnerRadius - minInnerRadius)/ baseDuration;
        
        while (_timeRemaining > 0)
        {
            _light.intensity             = Mathf.Max(minIntensity, _light.intensity - intensity * Time.deltaTime);
            _lightCollider.radius        = Math.Max(minRadius, _lightCollider.radius - radius * Time.deltaTime);
            _light.pointLightOuterRadius = Mathf.Max(minRadius, _light.pointLightOuterRadius - radius * Time.deltaTime);
            _light.pointLightInnerRadius = Mathf.Max(minInnerRadius, _light.pointLightInnerRadius - innerRadius * Time.deltaTime);
            _timeRemaining               = Mathf.Max(0, _timeRemaining - Time.deltaTime);
            yield return null;
        }
    }
    

    private void EnableLight()
    {
        _lightOn = true;
        OnLightToggled?.Invoke(_lightOn);
        _light.enabled = true;
        ResetCoroutine(DecrementLight());
    }

    private void ResetCoroutine(IEnumerator routine)
    {
        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
        }
        _currentRoutine = StartCoroutine(routine);
    }

    private void DisableLight()
    {
        _lightOn = false;
        OnLightToggled?.Invoke(_lightOn);
        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
        }
        _light.enabled = false;
    }

    private void ResetLight()
    {
        _timeRemaining = baseDuration;
        _light.intensity = baseIntensity;
        _lightCollider.radius = baseRadius;
        _light.pointLightInnerRadius = baseInnerRadius;
        _light.pointLightOuterRadius = baseRadius;
        DisableLight();

    }

    public bool IsLightOn()
    {
        return _lightOn;
    }
    
    
    public IEnumerator GraduallyRefillOil(float oil)
    {
        if (!_playerItemTracker.PlayerHasLamp()) yield break;
        
        refillingLamp.Invoke();
        var intensity = (baseIntensity / baseDuration) * oil;
        var radius = (baseRadius / baseDuration) * oil;
        var innerRad = (baseRadius - 1 / baseDuration) * oil;
        float clampedTime = Mathf.Clamp(_timeRemaining + oil, 0, baseDuration);
        while (_timeRemaining < clampedTime)
        {
            _light.pointLightInnerRadius = Mathf.Min(_light.pointLightInnerRadius + (innerRad * Time.deltaTime),
                baseRadius - 1);
            _light.intensity = Mathf.Min(_light.intensity + (intensity * Time.deltaTime), baseIntensity);
            _timeRemaining += clampedTime * Time.deltaTime;
            yield return null;
        }
    }

    public void OilRefill(float oil)
    {
        if (!_playerItemTracker.PlayerHasLamp()) return;
        
        var intensityToAdd = (baseIntensity / baseDuration) * oil;
        var radiusToAdd = (baseRadius / baseDuration) * oil;
        var innerRadToAdd = (baseRadius - 1 / baseDuration) * oil;
        float clampedTime = Mathf.Clamp(_timeRemaining + oil, 0, baseDuration);
            _light.pointLightInnerRadius = Mathf.Min(_light.pointLightInnerRadius + (innerRadToAdd),
                baseRadius - 1);
            _light.intensity = Mathf.Min(_light.intensity + radiusToAdd, baseIntensity);
            _timeRemaining = clampedTime;
    }
    
    

    public void PickedUpLamp()
    {
        ResetLight();
        DisableLight();
    }
}