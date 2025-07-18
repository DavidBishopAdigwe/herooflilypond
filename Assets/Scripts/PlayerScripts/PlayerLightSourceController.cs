using Enums;
using Singletons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace PlayerScripts
{
    public class PlayerLightSourceController : MonoBehaviour
    {
        private InputAction _playerLightAction;
        private LightSource _lightSource;

        private void Start()
        {
            InputReader.Instance.LightPerformed += OnLightKeyClicked;
            
            _lightSource = GetComponentInChildren<LightSource>();
            
        }

        private void OnLightKeyClicked(InputAction.CallbackContext obj)
        {
            _lightSource.ToggleLight();
        }
    
        private void OnDestroy()
        {
            InputReader.Instance.LightPerformed -= OnLightKeyClicked;
        }
    }
}


