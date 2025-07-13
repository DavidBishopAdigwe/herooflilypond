using System;
using System.Collections;
using System.Diagnostics;
using DataPersistence;
using DataPersistence.Data;
using Interfaces;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LightSource : MonoBehaviour, IDataPersistence
{
    
    [SerializeField] private Light2D whiteLight;
    [SerializeField] private Sprite lampSprite;
    [SerializeField] private Image lightUIImage;
    [Header("Base Settings")] 
    [SerializeField] private float baseDuration;
    [SerializeField] private float baseIntensity;
    [SerializeField] private float baseRadius;
    [SerializeField] private float baseInnerRadius;

    [Header("Minimum Settings")]
    [SerializeField] private float minIntensity;
    [SerializeField] private float minRadius;
    [SerializeField] private float minInnerRadius;
    [SerializeField] private int secondsToMaxRefill;

    [Header("UI Objects")] 
    [SerializeField] private Image lampFuelRenderer;
    [SerializeField] private Sprite[] lampFuelFrames;
    [SerializeField] private Sprite[] lampIconFrames;
    
    
    
    private CircleCollider2D _lightCollider;
    private Light2D _light;
    private float _timeRemaining;
    public event Action<bool> OnLightToggled;
    private Coroutine _currentRoutine;
    private bool _lightOn;
    private PlayerItemTracker _playerItemTracker;
    private bool _refilling;
    private float _baseColliderRadius;
    private float _minColliderRadius;
    private float _lightProgress;
    private const int LightToColliderRadiusMultiplier = 3; // Collider radius values scale to 3x of Light2D outer radius values.


    private void Awake()
    {
        _lightCollider = GetComponent<CircleCollider2D>();
        _light = GetComponent<Light2D>();
        _playerItemTracker = GetComponentInParent<PlayerItemTracker>();
        _baseColliderRadius = baseRadius * LightToColliderRadiusMultiplier;
        _minColliderRadius = minRadius * LightToColliderRadiusMultiplier;
    }

    private void Start()
    {
        DisableLight();
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
        var radius                  = (baseRadius -minRadius) / baseDuration; 
        var innerRadius             = (baseInnerRadius - minInnerRadius)/ baseDuration;
        var colliderRadius          = (_baseColliderRadius - _minColliderRadius) / baseDuration;
         
        while (_timeRemaining > 0)
        {
            
            _light.intensity             = Mathf.Max(minIntensity, _light.intensity - intensity * Time.deltaTime);
            _lightCollider.radius        = Mathf.Max(minRadius, _lightCollider.radius - colliderRadius * Time.deltaTime);
            _light.pointLightOuterRadius = Mathf.Max(minRadius, _light.pointLightOuterRadius - radius * Time.deltaTime);
            _light.pointLightInnerRadius = Mathf.Max(minInnerRadius, _light.pointLightInnerRadius - innerRadius * Time.deltaTime);
            SetWhiteLight();
            _timeRemaining               = Mathf.Max(0, _timeRemaining - Time.deltaTime);
            
            SetLampProgressSprites();
            if (_timeRemaining <= 0)
            {
                DisableLight();
            }
            yield return null;
        }
        
    }

    private void SetLampProgressSprites()
    {
        _lightProgress = 1 - (_timeRemaining / baseDuration);
        var fuelIndex = Mathf.FloorToInt(_lightProgress * (lampFuelFrames.Length - 1));
        fuelIndex = Mathf.Clamp(fuelIndex, 0, lampFuelFrames.Length - 1);
        lampFuelRenderer.sprite = lampFuelFrames[fuelIndex];
    }

    private void EnableLight()
    {
        if (_timeRemaining <= 0) return;
        _lightOn = true;
        OnLightToggled?.Invoke(_lightOn);
        _lightCollider.enabled = true;
        _light.enabled = true;
        whiteLight.enabled = true;
        SwitchCoroutine(DecrementLight());
    }

    public void DisableLight()
    {
        if (_refilling) return;
        _lightOn = false;
        OnLightToggled?.Invoke(_lightOn);
       if(_currentRoutine != null) StopCoroutine(_currentRoutine);
       _lightCollider.enabled = false;
        _light.enabled = false;
        whiteLight.enabled = false;

    }

    private void ResetLight()
    {
        _timeRemaining = baseDuration;
        _light.intensity = baseIntensity;
        _lightCollider.radius = _baseColliderRadius;
        _light.pointLightInnerRadius = baseInnerRadius;
        _light.pointLightOuterRadius = baseRadius;
        SetLampProgressSprites();
        DisableLight();

    }

    public bool IsLightOn()
    {
        return _lightOn;
    }

    private void SwitchCoroutine(IEnumerator newRoutine)
    {
        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
            _currentRoutine = null;
        }
        _currentRoutine = StartCoroutine(newRoutine);
    }
    

    public void OilRefillGradual( float oil)
    {
        SwitchCoroutine(GraduallyRefillOil(oil));
        
    }
    private IEnumerator GraduallyRefillOil(float oil)
    {  
        if (!_playerItemTracker.PlayerHasLamp()) yield break;

        _refilling = true;
        _lightOn = true;
        OnLightToggled?.Invoke(_lightOn);
        _light.enabled = true;
        
        oil = Mathf.Clamp(oil, 0, baseDuration);
        var intensity      = Mathf.Clamp(_light.intensity             + ((baseIntensity - minIntensity) / baseDuration)             
                                                                           * oil, 0, baseIntensity);
        
        var radius         = Mathf.Clamp(_light.pointLightOuterRadius + ((baseRadius - minRadius) / baseDuration)                  
                                                                           * oil, 0, baseRadius);
        
        var innerRad       = Mathf.Clamp(_light.pointLightInnerRadius +  ((baseInnerRadius - minInnerRadius) / baseDuration)        
                                                                           * oil, 0, baseInnerRadius);
        
        var colliderRadius = Mathf.Clamp(_lightCollider.radius        + ((_baseColliderRadius - _minColliderRadius) / baseDuration) 
                                                                           * oil, 0, _baseColliderRadius);
        
        float clampedTime = Mathf.Clamp(_timeRemaining + oil, 0, baseDuration);
        while (_timeRemaining < clampedTime)
        {
            _light.intensity = Mathf.MoveTowards(_light.intensity, intensity, Time.deltaTime);
            _light.pointLightOuterRadius = Mathf.MoveTowards(_light.pointLightOuterRadius, radius, Time.deltaTime);
            _light.pointLightInnerRadius = Mathf.MoveTowards(_light.pointLightInnerRadius, innerRad, Time.deltaTime);
            _lightCollider.radius = Mathf.MoveTowards(_lightCollider.radius, colliderRadius, Time.deltaTime); 
            _timeRemaining              += clampedTime * (Time.deltaTime) ;
            SetLampProgressSprites();
            if (_timeRemaining >= clampedTime)
            {
                _timeRemaining = clampedTime;
                _refilling = false;
                SetWhiteLight();
                SwitchCoroutine(DecrementLight());
            }   
            yield return null;
        } 
    }

    private void SetWhiteLight()
    {
        whiteLight.pointLightOuterRadius = _light.pointLightOuterRadius;
        whiteLight.intensity = _light.intensity;
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
    }

    public void LoadData(GameData data)
    {
        _light.intensity = data.lightIntensity;
        _lightCollider.radius = data.lightRadius;
        _light.pointLightOuterRadius = data.lightRadius;
        _light.pointLightOuterRadius = data.lightInnerRadius;
        _timeRemaining = data.lightTimeRemaining;
    }

    public void SaveData(ref GameData data)
    {
        data.lightIntensity = _light.intensity;
        data.lightRadius = _light.pointLightOuterRadius;
        data.lightInnerRadius = _light.pointLightInnerRadius;
        data.lightTimeRemaining = _timeRemaining;
    }


}