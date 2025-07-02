using Enums;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace PlayerScripts
{
    public class PlayerLightSourceController : MonoBehaviour
    {
        private InputAction _playerLightAction;
        private Lamp _attachedLightSource;

        private void Start()
        {
            _attachedLightSource = GetComponentInChildren<Lamp>();

            _playerLightAction = InputManager.Instance.GetLightAction();
            _playerLightAction.Enable();

            _playerLightAction.performed += OnLightKeyClicked;
        }

        private void OnLightKeyClicked(InputAction.CallbackContext obj)
        {
                _attachedLightSource.ToggleLight();
        }



    
        private void OnDestroy()
        {
            _playerLightAction.performed -= OnLightKeyClicked;
      
            _playerLightAction.Disable();
        }
    }
}


