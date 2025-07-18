using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Singletons
{
    [CreateAssetMenu(fileName = "Input Manager", menuName = "ScriptableObjects/InputManager")]
    public class InputManager : ScriptableObjectSingleton<InputManager>
    {
        private InputSystem_Actions _inputSystem;

        private void OnEnable()
        {
            _inputSystem = new InputSystem_Actions();
        } 
        
    public InputAction GetMoveAction() => _inputSystem.Player.Move;

    public InputAction GetLightAction() => _inputSystem.Player.Light;
    
    public InputAction GetInteractAction() => _inputSystem.Player.Pick;
    
    public InputAction GetBackAction() => _inputSystem.Player.Back;
    

    }
}
