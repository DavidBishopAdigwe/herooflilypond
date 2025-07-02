using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace DeprecatedScripts
{
    public class DeprecatedLightSource : MonoBehaviour
    {
        private static readonly int PlayerLightPos = Shader.PropertyToID("_PlayerLightPos");
        private static readonly int PlayerLightRadius = Shader.PropertyToID("_PlayerLightRadius");

        [Header("General Settings")] [SerializeField]
        private LightUI lightUI;

        [SerializeField] private Image currentLightSource;

        [SerializeField] private float minimumRadiusSize;
        [SerializeField] private float minimumIntensity;
        [SerializeField] private float minimumInnerRadiusSize;
        [SerializeField] private Sprite lampSprite;
        [SerializeField] private Sprite torchSprite;

        private Light2D _light;
        private CircleCollider2D _collider;

        public LightSourceType typeOfLight = LightSourceType.None;


        public float timeRemaining;
        private Coroutine _lightCoroutine;
        private bool _lightOn;
        private bool _oneTimeDone;
        private float _baseIntensity, _baseInnerRadius, _baseOuterRadius, _baseColliderRadius;
        private float _lampIntensity, _lampInner, _lampOuter, _lampRadius, _lampTime;
        private bool _lampWasOn;
        public bool hasLamp;
        private bool _hasTorchToSwapTo;
        private bool _hasLampToSwapTo;
        private float _torchIntensity, _torchInner, _torchOuter, _torchRadius, _torchTime;
        private bool _torchWasOn;
        public bool hasTorch;
        private bool _lightCooldownIsActive;
        private float _coroutineCooldownValue;
        private Color32 _fireColour;
    

        [Header(" Lamp Settings")] // base values, might change to const

        [SerializeField]
        public float lampIntensity;

        [SerializeField] public float lampInnerRadius;
        [SerializeField] public float lampOuterRadius;
        [SerializeField] public float lampColliderRadius;
        [SerializeField] public float lampDuration;

        [Header(" Torch Settings")] [SerializeField]
        public float torchIntensity;
        [SerializeField] public float torchInnerRadius;
        [SerializeField] public float torchOuterRadius;
        [SerializeField] public float torchColliderRadius;
        [SerializeField] public float torchDuration;
    
        [SerializeField] public Transform player;
    
    

        private void Awake()
        {
            _light = GetComponent<Light2D>();
            _collider = GetComponent<CircleCollider2D>();
            _fireColour = new Color32(255, 119, 0, 255); 
        }


        public void ToggleLight()
        {
            _lightOn = !_lightOn;

            if (_lightOn)
            {
                EnableLight();
            }
            else
            {
                DisableLight();
            }
        }

        private void Start()
        {
            NoLightSettings();

            player = GetComponentInParent<Transform>();
        }

        private void NoLightSettings()
        {
            if (typeOfLight == LightSourceType.None)
            {
                currentLightSource.color = Color.white;
            }
        }

        private void EnableLight()
        {
            if (timeRemaining <= 0) return;
        
            _light.enabled = true;
            _collider.enabled = true;
            lightUI.GetUIDuration(GetDuration());

            if (_lightCoroutine != null)
                StopCoroutine(_lightCoroutine);

            _lightCoroutine = StartCoroutine(LightCooldown());
        }

        private void DisableLight()
        {
            _light.enabled = false;
            _collider.enabled = false;
            _lightOn = false;

            if (_lightCoroutine != null)
            {
                StopCoroutine(_lightCoroutine);
                _lightCoroutine = null;
            }
        }

        public void ChangeLightType(LightSourceType type)
        {
            typeOfLight = type;
        }

        public float TimeRemainingUI()
        {
            return timeRemaining;
        }

        private void OneTimeEvents()
        {
            if (_oneTimeDone) return;

            timeRemaining = typeOfLight switch
            {
                LightSourceType.Lamp => lampDuration,
                LightSourceType.Torch => torchDuration,
                _ => timeRemaining
            };

            _oneTimeDone = true;
        }

        private void LightProperty()
        {

            switch (typeOfLight)
            {
                case LightSourceType.Lamp when hasLamp && _lampTime > 0:
                    LightSettings(_lampIntensity, _lampInner, _lampOuter, _lampRadius, Color.yellow);
                    currentLightSource.sprite = lampSprite;
                    timeRemaining = _lampTime;
                    if (_lampWasOn) DisableLight();
                    break;
                case LightSourceType.Lamp:
                    LightSettings(lampIntensity, lampInnerRadius, lampOuterRadius, lampColliderRadius, Color.yellow);
                    currentLightSource.sprite = lampSprite;
                    timeRemaining = lampDuration;
                
                    break;
                case LightSourceType.Torch when hasTorch && _torchTime > 0:
                    LightSettings(_torchIntensity, _torchInner, _torchOuter, _torchRadius, _fireColour);
                    currentLightSource.sprite = torchSprite;
                    timeRemaining = _torchTime;
                    if (_torchWasOn) DisableLight();
                    break;
                case LightSourceType.Torch:
                    LightSettings(torchIntensity, torchInnerRadius, torchOuterRadius, torchColliderRadius, _fireColour);
                    currentLightSource.sprite = torchSprite;
                    timeRemaining = torchDuration;
                    break;

                case LightSourceType.None:
                    break;

            }


            _baseIntensity = _light.intensity;
            _baseInnerRadius = _light.pointLightInnerRadius;
            _baseOuterRadius = _light.pointLightOuterRadius;
            _baseColliderRadius = _collider.radius;
        }

        private void LightSettings(float intensity, float innerRadius, float outerRadius, float colliderRadius, Color color)
        {
            _light.color = color;
            _light.intensity = intensity;
            _light.pointLightInnerRadius = innerRadius;
            _light.pointLightOuterRadius = outerRadius;
            _collider.radius = colliderRadius;
        }

        private float GetDuration()
        {
            return typeOfLight switch
            {
                LightSourceType.Lamp => lampDuration,
                LightSourceType.Torch => torchDuration,
                _ => 0
            };
        }

        private IEnumerator LightCooldown()
        {
            float duration = GetDuration();
            float intensityStep = (_baseIntensity - minimumIntensity) / duration;
            float innerStep = (_baseInnerRadius - minimumInnerRadiusSize)  / duration;
            float outerStep = (_baseOuterRadius - minimumRadiusSize)/ duration;
            float radiusStep = (_baseColliderRadius - minimumRadiusSize) / duration;

            while (timeRemaining > 0)
            {
                if (_lightOn)
                {
                    _coroutineCooldownValue = _light.intensity < lampIntensity / 3f ? 1f : 0.5f;
                
                    _light.intensity = Mathf.Max(minimumIntensity, _light.intensity - intensityStep);
                    _light.pointLightInnerRadius = Mathf.Max(minimumInnerRadiusSize, _light.pointLightInnerRadius - innerStep);
                    _light.pointLightOuterRadius = Mathf.Max(minimumRadiusSize, _light.pointLightOuterRadius - outerStep);
                    _collider.radius = Mathf.Max(minimumRadiusSize, _collider.radius - radiusStep);
                
                    Shader.SetGlobalVector(PlayerLightPos, player.position);
                    Shader.SetGlobalFloat(PlayerLightRadius, _light.pointLightOuterRadius);
                
                
                    lightUI.ChangeSize(timeRemaining);
                    timeRemaining--;
                    if (_light.intensity > lampIntensity / 3f) // 3 and 5 are arbitrary, gives light flickering effect
                    {
                        _light.intensity = Mathf.Max(intensityStep * 5f, _light.intensity - intensityStep * 5f);
                        yield return new WaitForSeconds(_coroutineCooldownValue);
                        _light.intensity = Mathf.Max(intensityStep * 5f, _light.intensity + intensityStep * 5f);
                    }

                    yield return new WaitForSeconds(_coroutineCooldownValue);

                    if (timeRemaining <= 0)
                    {
                        _light.intensity = 0;
                        _light.pointLightInnerRadius = 0;
                        _light.pointLightOuterRadius = 0;
                        _collider.radius = 0;
                        DisableLight();
                    }
                    switch (typeOfLight)
                    {
                        case LightSourceType.Lamp when timeRemaining <= 0:
                            SwapLight();
                            _hasLampToSwapTo = false;
                            break;
                        case LightSourceType.Torch when timeRemaining <= 0:
                            SwapLight();
                            _hasTorchToSwapTo = false;
                            break;
                        case LightSourceType.None:
                            break;
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

        public void PickupLight()
        {
            DisableLight();
            _oneTimeDone = false;
            OneTimeEvents();
            LightProperty();
            lightUI.GetUIDuration(GetDuration());
        }

        public void SwapLight()
        {
            if ((typeOfLight == LightSourceType.Lamp && !_hasTorchToSwapTo) ||
                (typeOfLight == LightSourceType.Torch && !_hasLampToSwapTo)) return;

            switch (typeOfLight)
            {
                case LightSourceType.Lamp:
                    KeepLampState();
                    typeOfLight = LightSourceType.Torch;
                    break;
                case LightSourceType.Torch:
                    KeepTorchState();
                    typeOfLight = LightSourceType.Lamp;
                    break;
                case LightSourceType.None:
                    break;
            }

            ResetAndDisable();
            LightProperty();

            lightUI.GetUIDuration(GetDuration());
        }

        private void KeepLampState()
        {
            _lampIntensity = _light.intensity;
            _lampInner = _light.pointLightInnerRadius;
            _lampOuter = _light.pointLightOuterRadius;
            _lampRadius = _collider.radius;
            _lampTime = timeRemaining;
            _lampWasOn = _lightOn;
        }

        private void KeepTorchState()
        {
            _torchIntensity = _light.intensity;
            _torchInner = _light.pointLightInnerRadius;
            _torchOuter = _light.pointLightOuterRadius;
            _torchRadius = _collider.radius;
            _torchTime = timeRemaining;
            _torchWasOn = _lightOn;
        }

        public void OilRefill(float oilAmount)
        {
        
            if (!hasLamp) return;
        
            _hasLampToSwapTo = true;
            if (typeOfLight == LightSourceType.Torch)
            {
                SwapLight();
            }
            float duration = lampDuration;
            float intensityStep = (lampIntensity - minimumIntensity) / duration;
            float innerStep = (lampInnerRadius - minimumInnerRadiusSize)  / duration;
            float outerStep = (lampOuterRadius - minimumRadiusSize)/ duration;
            float radiusStep = (lampColliderRadius - minimumRadiusSize) / duration;
            
            KeepLampState();
            _lampIntensity = Mathf.Clamp(_lampIntensity + intensityStep * oilAmount, minimumIntensity, lampIntensity);
            _lampInner = Mathf.Clamp(_lampInner + innerStep * oilAmount, minimumInnerRadiusSize, lampInnerRadius);
            _lampOuter = Mathf.Clamp(_lampInner + outerStep * oilAmount, minimumRadiusSize, lampOuterRadius);
            _lampRadius = Mathf.Clamp(_lampInner + radiusStep * oilAmount , minimumRadiusSize, lampColliderRadius);
            _lampTime = Mathf.Clamp(_lampTime + oilAmount, 0, lampDuration);
            _hasLampToSwapTo = true;
            
            LightProperty();
            lightUI.ChangeSize(timeRemaining);
            DisableLight();
        
        }

        public void GetLamp(bool lampGetter)
        {
            hasLamp = lampGetter;
            _hasLampToSwapTo = lampGetter;
        }

        public void GetTorch(bool torchGetter)
        {
            hasTorch = torchGetter;
            _hasTorchToSwapTo = torchGetter;
        }
    }
}

