using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Singletons;

[CreateAssetMenu(menuName = "ScriptableObjects/InputReader", fileName = "InputReader")]
public class InputReader : ScriptableObjectSingleton<InputReader>
{
    public event Action<InputAction.CallbackContext> MovePerformed;
    public event Action<InputAction.CallbackContext> MoveCanceled;
    public event Action UnsubscribeMove;
    public event Action SubscribeMove;
    
    public event Action<InputAction.CallbackContext> InteractPerformed;
    public event Action UnsubscribeInteract;
    public event Action SubscribeInteract;


    public event Action<InputAction.CallbackContext> LightPerformed; 
    public event Action UnsubscribeLight;
    public event Action SubscribeLight;


    public event Action<InputAction.CallbackContext> BackPerformed;
    
    public event Action UnsubscribeBack;
    public event Action SubscribeBack;
    


    
    
    private InputAction _move;
    private InputAction _interact;
    private InputAction _light;
    private InputAction _back;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubscribeAllActions();  
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnsubscribeAllActions();  
        Subscribe();
    }

    private void Subscribe()
    {
        _move     = InputManager.Instance.GetMoveAction(); 
        _interact = InputManager.Instance.GetInteractAction();
        _light    = InputManager.Instance.GetLightAction();
        _back     = InputManager.Instance.GetBackAction();

        _move.Enable();
        _move.performed += OnMovePerformed;
        _move.canceled  += OnMoveCanceled;
        
        _interact.Enable();
        _interact.performed += OnInteractPerformed;
        
        _light.Enable();
        _light.performed += OnLightPerformed;

        _back.Enable();
        _back.performed += OnBackPerformed;
    }

    public void UnsubscribeAllActions()
    {
        UnsubscribeMoveAction(); 
        UnsubscribeInteractAction(); 
        UnsubscribeLightAction();
        UnsubscribeBackAction();
    }

    public void UnsubscribeMoveAction()
    {
        if (_move != null)
        {
            _move.performed -= OnMovePerformed;
            _move.canceled -= OnMoveCanceled;
            _move.Disable();
            UnsubscribeMove?.Invoke();
        }
        _move = null;
    }

    public void SubscribeMoveAction()
    {
        _move     = InputManager.Instance.GetMoveAction();
        
        _move.Enable();
        _move.performed += OnMovePerformed;
        _move.canceled  += OnMoveCanceled;
        SubscribeMove?.Invoke();
    }

    public void UnsubscribeInteractAction()
    {
        if (_interact != null)
        {
            _interact.performed -= OnInteractPerformed;
            _interact.Disable();
            
            UnsubscribeInteract?.Invoke();
        }
        _interact  = null;

    }

    public void SubscribeInteractAction()
    {
        _interact = InputManager.Instance.GetInteractAction();
        
        _interact.Enable();
        _interact.performed += OnInteractPerformed;
        SubscribeInteract?.Invoke();
    }

    public void SubscribeLightAction()
    {
        _light   = InputManager.Instance.GetLightAction();
        
        _light.Enable();
        _light.performed += OnLightPerformed;
        SubscribeLight?.Invoke();
    }

    public void UnsubscribeLightAction()
    {
        if (_light != null)
        {
            _light.performed -= OnLightPerformed;
            _light.Disable();
            
            UnsubscribeLight?.Invoke();
        }
        _light  = null;

    }

    public void SubscribeBackAction()
    {
        _back    = InputManager.Instance.GetBackAction();
        
        _back.Enable();
        _back.performed += OnBackPerformed;
        
        SubscribeBack?.Invoke();
    }

    public void UnsubscribeBackAction()
    {
        if (_back != null)
        {
            _back.performed -= OnBackPerformed;
            _back.Disable();
            UnsubscribeBack?.Invoke();
        }
        _back = null;
    }
    
    
    private void OnMovePerformed(InputAction.CallbackContext callback) => MovePerformed?.Invoke(callback);

    private void OnMoveCanceled(InputAction.CallbackContext callback) => MoveCanceled?.Invoke(callback);
    
    private void OnBackPerformed(InputAction.CallbackContext callback) => BackPerformed?.Invoke(callback);

    private void OnInteractPerformed(InputAction.CallbackContext callback) => InteractPerformed?.Invoke(callback);
    
    private void OnLightPerformed(InputAction.CallbackContext callback) => LightPerformed?.Invoke(callback);


}
