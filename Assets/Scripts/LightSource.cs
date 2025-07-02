using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class LightSource : MonoBehaviour
{

    [SerializeField] private LightUI lightUI;
    [SerializeField] private Image currentLightSource;
    [SerializeField] private float minimumRadiusSize;
    [SerializeField] private float minimumIntensity;
    [SerializeField] private float minimumInnerRadiusSize;
    [SerializeField] private Vector3 minimumScale;
    [SerializeField] private Sprite lampSprite;
    [SerializeField] private Sprite torchSprite;
    
    [Header(" Lamp Settings")] 

    [SerializeField] private float lampIntensity;
    [SerializeField] private float lampColliderRadius;
    [SerializeField] private float lampDuration;
    [SerializeField] private Vector3 lampScale;

    [Header(" Torch Settings")]
    
    [SerializeField] private float torchIntensity;
    [SerializeField] private float torchColliderRadius;
    [SerializeField] private float torchDuration;
    [SerializeField] private Vector3 torchScale;
    
    private Light2D _light;
    private CircleCollider2D _collider;
    private Animator _animator;

    public LightSourceType TypeOfLight { get; private set; }

    private float _timeRemaining;
    private Coroutine _lightCoroutine;
    private Coroutine _animationCoroutine;
    private bool _lightOn;
    
    private Vector3 _baseScale;
    private float _baseIntensity, _baseInnerRadius, _baseOuterRadius, _baseColliderRadius; 
    
    private float _storedLampIntensity,_storedLampRadius, _storedLampTime;
    private Vector3 _storedLampScale;
    
    private float _storedTorchIntensity, _storedTorchRadius, _storedTorchTime;
    private Vector3 _storedTorchScale;
    
    private Color32 _fireColour;
    public event Action<bool> OnLightToggled; 
    private SpriteRenderer _spriteRenderer;
    private Queue<LightSourceType> _lightQueue = new ();
    
    
    

    private void Awake()
    {
       _light = GetComponent<Light2D>();
        _collider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _fireColour = new Color32(255, 119, 0, 255); 
    }


    public void ToggleLight()
    {
        if (_timeRemaining <= 0) return;
        _lightOn = !_lightOn;
        if (_lightOn)
            EnableLight();
        else
            DisableLight();
    }

    private void Start()
    {
        BaseLightProperty();
        if (TypeOfLight != LightSourceType.None)
        {
            _lightQueue.Enqueue(TypeOfLight);
        }
    }
    

    private void EnableLight()
    {
       
        if (_timeRemaining <= 0) return;
        
        bool lightWasOff = !_light.enabled;
    
        _light.enabled = true;
        _collider.enabled = true;
        
        lightUI.GetUIDuration(GetDuration());

        if (_lightCoroutine != null)
            StopCoroutine(_lightCoroutine);
        
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(LightAnimation());

        _lightCoroutine = StartCoroutine(LightCooldown());
        
        if (lightWasOff)
        {
            OnLightToggled?.Invoke(true);
        }
    }

    public bool GetLightOn()
    {
        return _lightOn;
    }

    public void DisableLight()
    {
        bool lightWasOn = _light.enabled;
        
        _light.enabled = false;
        _collider.enabled = false;
        _lightOn = false;

        if (_lightCoroutine != null)
        {
            StopCoroutine(_lightCoroutine);
            _lightCoroutine = null;
        }
        
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
        
        if (lightWasOn)
        {
            OnLightToggled?.Invoke(false);
        }
        
        
    }
    
    private IEnumerator LightAnimation() 
    {
        while (_lightOn)
        {
            if (_spriteRenderer) _light.lightCookieSprite = _spriteRenderer.sprite;
            
            yield return null;
        }
    }

    private void ChangeLightType(LightSourceType type)
    {
        switch (TypeOfLight)
        {
            case LightSourceType.Lamp:
                StoreLampSettings();
                break;
            case LightSourceType.Torch:
                StoreTorchSettings();
                break;
            case LightSourceType.None:
                break;
        }
        TypeOfLight = type;
    }
    
    
    private void StoredLightProperty() 
    {

        switch (TypeOfLight)
        {
            case LightSourceType.Lamp:
                LightSettings(_storedLampIntensity, _storedLampScale, _storedLampRadius, Color.yellow, _storedLampTime, lampSprite);
                ChangeBaseLightSettings(lampIntensity, lampScale, lampColliderRadius);
                if (_lightOn) DisableLight();
                break;
            case LightSourceType.Torch:
                LightSettings(_storedTorchIntensity, _storedTorchScale, _storedTorchRadius, _fireColour, _storedTorchTime, torchSprite);
                ChangeBaseLightSettings(torchIntensity, torchScale, torchColliderRadius);
                if (_lightOn) DisableLight();
                break;
            case LightSourceType.None:
                LightSettings(0, Vector3.zero, 0, Color.white, 0, null);
                ChangeBaseLightSettings(0, Vector3.zero, 0);
                if (_lightOn) DisableLight();
                break;

        }
        
    }
    

    
    private void BaseLightProperty()
    {

        switch (TypeOfLight)
        {
           
            case LightSourceType.Lamp:
                LightSettings(lampIntensity, lampScale, lampColliderRadius, Color.yellow, lampDuration, lampSprite);
                ChangeBaseLightSettings(lampIntensity, lampScale, lampColliderRadius);
                if(_lightOn) DisableLight();
                break;
            case LightSourceType.Torch:
                LightSettings(torchIntensity, torchScale, torchColliderRadius, _fireColour, torchDuration, torchSprite);
                ChangeBaseLightSettings(torchIntensity, torchScale, torchColliderRadius);
                if (_lightOn) DisableLight();
                break;
            case LightSourceType.None:
                LightSettings(0, Vector3.zero, 0, Color.white, 0, null);
                ChangeBaseLightSettings(0, Vector3.zero, 0);
                if (_lightOn) DisableLight();
                break;

        }

        
    }
    
    private void ChangeBaseLightSettings(float intensity, Vector3 scale, float colliderRadius) 
    {
        _baseIntensity = intensity;
        _baseScale = scale;
        _baseColliderRadius = colliderRadius;
    }

    private void LightSettings(float intensity, Vector3 scale, float colliderRadius, Color color, float time, Sprite sprite)
    {
        _light.color = color;
        _light.intensity = intensity;
        _light.gameObject.transform.localScale = scale;
        _collider.radius = colliderRadius;
        _timeRemaining = time;
        currentLightSource.sprite = sprite;
    }
    
   

    private float GetDuration()
    {
        return TypeOfLight switch
        {
            LightSourceType.Lamp => lampDuration,
            LightSourceType.Torch => torchDuration,
            _ => 0
        };
    }

    private IEnumerator LightCooldown()
    {
        float duration = GetDuration();
        float intensityStep = (_baseIntensity - minimumIntensity) * Time.deltaTime / duration;
        Vector3 scaleStep = (_baseScale - minimumScale ) * Time.deltaTime / duration;
        float radiusStep = (_baseColliderRadius - minimumRadiusSize) * Time.deltaTime / duration;

        while (_timeRemaining > 0)
        {
            if (_lightOn)
            {
                
                _light.intensity = Mathf.Max(minimumIntensity, _light.intensity - intensityStep) ;
                _light.gameObject.transform.localScale = new Vector3(
                    Mathf.Max(minimumScale.x, _light.gameObject.transform.localScale.x - scaleStep.x),
                    Mathf.Max(minimumScale.y, _light.gameObject.transform.localScale.y - scaleStep.y), 1f);
                _collider.radius = Mathf.Max(minimumRadiusSize, _collider.radius - radiusStep);
                
                lightUI.ChangeSize(_timeRemaining);
                _timeRemaining -= Time.deltaTime;
                
                yield return new WaitForSeconds(1*Time.deltaTime);

                if (_timeRemaining <= 0)
                {
                    _light.intensity = 0;
                    _light.transform.localScale = Vector3.zero;
                    _collider.radius = 0;
                    if (_lightQueue.Count > 0) _lightQueue.Dequeue();
                    DisableLight();
                    SwapLight();
                }
            }

            yield return null;


        }
    }

    public void ResetAndDisable()
    {
        if (_lightCoroutine != null)
        {
            StopCoroutine(_lightCoroutine);
            _lightCoroutine = null;
        }

        _lightOn = false;
        DisableLight();
    }

    public void PickupLight(LightSourceType lightType)
    {
        StoreCurrentLightSettings();
        ChangeLightType(lightType);
        DisableLight();
        if (!_lightQueue.Contains(TypeOfLight))
        {
            _lightQueue.Enqueue(TypeOfLight);
        }
        BaseLightProperty();
        lightUI.GetUIDuration(GetDuration());
        lightUI.ChangeSize(_timeRemaining);
    }
    public void SwapLight()
    {
       if (_lightQueue.Count <= 1) return;
       
        StoreCurrentLightSettings();
        
        var previousLightType = _lightQueue.Dequeue();
        TypeOfLight = _lightQueue.Peek();
        _lightQueue.Enqueue(previousLightType);
        
        ResetAndDisable();
        StoredLightProperty();

        lightUI.GetUIDuration(GetDuration());
        lightUI.ChangeSize(_timeRemaining);
    }

    private void StoreCurrentLightSettings()
    {
        switch (TypeOfLight)
        {
            case LightSourceType.Lamp:
                StoreLampSettings();
                break;
            case LightSourceType.Torch:
                StoreTorchSettings();
                break;
            case LightSourceType.None:
                break;
        }
    }
    
    private void StoreLampSettings()
    {
        _storedLampIntensity = _light.intensity;
        _storedLampScale = _light.gameObject.transform.localScale;
        _storedLampRadius = _collider.radius;
        _storedLampTime = _timeRemaining;
    }

    private void StoreTorchSettings()
    {
        _storedTorchIntensity = _light.intensity;
        _storedTorchScale = _light.gameObject.transform.localScale;
        _storedTorchRadius = _collider.radius;
        _storedTorchTime = _timeRemaining;
    }



    public void OilRefill(float oilAmount) 
    {
            if (!_lightQueue.Contains(LightSourceType.Lamp)) // Adds lamp to queue if it was empty
            {
                _lightQueue.Enqueue(LightSourceType.Lamp);
            }
            
            int maxIterations = _lightQueue.Count;
            bool foundLamp = false;

            while (maxIterations-- > 0 && !foundLamp)
            {
                if (_lightQueue.Peek() == LightSourceType.Lamp) 
                {
                    StoreCurrentLightSettings();
                    TypeOfLight = LightSourceType.Lamp; 
                    foundLamp = true;
                    float duration = lampDuration;
                    float intensityStep = (lampIntensity - minimumIntensity) / duration;
                    Vector3 scaleStep = (lampScale - minimumScale) / duration;
                    float radiusStep = (lampColliderRadius - minimumRadiusSize) / duration;

                    _storedLampIntensity = Mathf.Clamp(_storedLampIntensity + intensityStep * oilAmount, minimumIntensity, lampIntensity);
                    _storedLampScale = new Vector3(Mathf.Clamp(_storedLampScale.x + scaleStep.x * oilAmount, minimumScale.x, lampScale.x), Mathf.Clamp(_storedLampScale.y + scaleStep.y * oilAmount, minimumScale.y, lampScale.y), 1f);
                    _storedLampRadius = Mathf.Clamp(_storedLampRadius + radiusStep * oilAmount, minimumRadiusSize, lampColliderRadius);
                    _storedLampTime = Mathf.Clamp(_storedLampTime + oilAmount, 0, lampDuration);
                    StoredLightProperty();
                    lightUI.ChangeSize(_timeRemaining);
                }
                else
                {
                    var previousLightType = _lightQueue.Dequeue();
                    _lightQueue.Enqueue(previousLightType);
                }
            }
            
    }
 

    
}

