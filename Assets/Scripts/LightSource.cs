using System;
using System.Collections;
using Managers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

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
    [SerializeField] private int secondsToMaxRefill;

    [Header("UI Objects")] 
    [SerializeField] private Image lampFuelRenderer;
    [SerializeField] private Image lampIconRenderer;
    [SerializeField] private Sprite[] lampFuelFrames;
    [SerializeField] private Sprite lampSprite;

    [SerializeField] private AudioClip turnOnSound;
    [SerializeField] private AudioClip turnOffSound;

    private Animator _animator;
    private CircleCollider2D _lightCollider;
    private Light2D _light;
    private float _timeRemaining;
    public event Action<bool> OnLightToggled;
    private Coroutine _decrementRoutine;
    private Coroutine _refillRoutine;
    private bool _lightOn;
    private PlayerItemTracker _playerItemTracker;
    private bool _refilling;
    private float _lightProgress;
    private AudioSource _audioSource;

    private AudioClip _currentSound;

    private void Awake()
    {
        _lightCollider = GetComponent<CircleCollider2D>();
        _light = GetComponent<Light2D>();
        _playerItemTracker = GetComponentInParent<PlayerItemTracker>();
        _audioSource = GetComponent<AudioSource>();
        _animator = _playerItemTracker.gameObject.GetComponent<Animator>();
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
        while (_timeRemaining > 0)
        {
            float currentFuelRatio = _timeRemaining / baseDuration;
            
            _light.intensity = minIntensity + (baseIntensity - minIntensity) * currentFuelRatio;
            _light.pointLightOuterRadius = minRadius + (baseRadius - minRadius) * currentFuelRatio;
            _light.pointLightInnerRadius = minInnerRadius + (baseInnerRadius - minInnerRadius) * currentFuelRatio;
            _lightCollider.radius = _light.pointLightOuterRadius;
            
            _timeRemaining -= Time.deltaTime;
            SetFuelProgressSprites();
            
            if (_timeRemaining <= 0)
            {
                DisableLight();
                yield break;
            }
            
            yield return null;
        }
    }

    private void SetFuelProgressSprites()
    {
        _lightProgress = 1 - (_timeRemaining / baseDuration);
        int fuelIndex = Mathf.FloorToInt(_lightProgress * (lampFuelFrames.Length - 1));
        lampFuelRenderer.sprite = lampFuelFrames[Mathf.Clamp(fuelIndex, 0, lampFuelFrames.Length - 1)];
    }

    private void EnableLight()
    {
        if (_timeRemaining <= 0) return;
        
        _lightOn = true; 
        _animator.SetBool("LightOn", true);
        OnLightToggled?.Invoke(_lightOn);
        _lightCollider.enabled = true;
        _light.enabled = true;
        _audioSource.PlayOneShot(turnOnSound);
        _audioSource.Play();
  
        if (!GameManager.Instance.PlayerInTutorial() && !_refilling)
        {
            StartDecrementRoutine();
        }
    }

    public void DisableLight()
    {
        if (_refilling) return;
        
        _lightOn = false;
        _animator.SetBool("LightOn", false);
        OnLightToggled?.Invoke(_lightOn);
        StopDecrementRoutine();
        
        _lightCollider.enabled = false;
        _light.enabled = false;
        _audioSource.Stop();
    }

    private void ResetLightSettings()
    {
        _timeRemaining = baseDuration;
        _light.intensity = baseIntensity;
        _lightCollider.radius = baseRadius;
        _light.pointLightInnerRadius = baseInnerRadius;
        _light.pointLightOuterRadius = baseRadius;
        lampIconRenderer.sprite = lampSprite;
        
        var spriteColour = lampFuelRenderer.color;
        spriteColour.a = 1f;
        lampIconRenderer.color = spriteColour;
        lampFuelRenderer.color = spriteColour;
        SetFuelProgressSprites();
        DisableLight();
    }

    private void StartDecrementRoutine()
    {
        if (_decrementRoutine != null) return;
        _decrementRoutine = StartCoroutine(DecrementLight());
    }

    private void StopDecrementRoutine()
    {
        if (_decrementRoutine != null)
        {
            StopCoroutine(_decrementRoutine);
            _decrementRoutine = null;
        }
    }

    public void OilRefillGradual(float oil)
    {
        if (_refillRoutine != null) StopCoroutine(_refillRoutine);
        _refillRoutine = StartCoroutine(GraduallyRefillOil(oil));
    }

    private IEnumerator GraduallyRefillOil(float oil)
    {  
        if (!_playerItemTracker.PlayerHasLamp() || _refilling) yield break;

        _refilling = true;
        EnableLight();
        if (_timeRemaining >= baseDuration - 0.1f)
        {
            _refilling = false;
            yield break;
        }

        float targetTime = Mathf.Clamp(_timeRemaining + oil, 0, baseDuration);
        float startTime = _timeRemaining;
        float targetIntensity = minIntensity + (baseIntensity - minIntensity) * (targetTime / baseDuration);
        float targetRadius = minRadius + (baseRadius - minRadius) * (targetTime / baseDuration);
        float targetInnerRadius = minInnerRadius + (baseInnerRadius - minInnerRadius) * (targetTime / baseDuration);
        float duration =  (oil / baseDuration);
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float incrementAmount = timeElapsed / duration;

            _timeRemaining = Mathf.Lerp(startTime, targetTime, incrementAmount);
            _light.intensity = Mathf.Lerp(_light.intensity, targetIntensity, incrementAmount);
            _light.pointLightOuterRadius = Mathf.Lerp(_light.pointLightOuterRadius, targetRadius, incrementAmount);
            _light.pointLightInnerRadius = Mathf.Lerp(_light.pointLightInnerRadius, targetInnerRadius, incrementAmount);
            SetFuelProgressSprites();
            yield return null;
        }

        _timeRemaining = targetTime;
        _light.intensity = targetIntensity;
        _light.pointLightOuterRadius = targetRadius;
        _light.pointLightInnerRadius = targetInnerRadius;
        _lightCollider.radius = targetRadius;
        SetFuelProgressSprites();

        _refilling = false;
    
        if (_decrementRoutine != null) 
        {
            StartDecrementRoutine();
        }
    }

    public void PickedUpLamp()
    {
        ResetLightSettings();
    }
}