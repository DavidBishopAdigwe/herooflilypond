using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    [CreateAssetMenu(fileName = "Input Manager", menuName = "ScriptableObjects/InputManager")]
    public class InputManager : ScriptableObjectSingleton<InputManager>
    {
        private InputSystem_Actions _inputSystem;

        private void OnEnable()
        {
            _inputSystem = new InputSystem_Actions();
        }

        public InputAction GetMoveAction()
    {
        return _inputSystem.Player.Move;
    }

    public InputAction GetLightAction()
    {
        return _inputSystem.Player.Light;
    }

    public InputAction GetDragAction()
    {
        return _inputSystem.Player.Drag;
    }

    public InputAction GetInteractAction()
    {
        return _inputSystem.Player.Pick;
    }

    public InputAction GetHideAction()
    {
        return _inputSystem.Player.Hide;
    }

    public InputAction GetPauseAction()
    {
        return _inputSystem.Player.Pause;
    }
    

    }
}
