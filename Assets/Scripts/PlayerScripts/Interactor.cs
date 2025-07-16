using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Managers;
using PickableItems;
using PlayerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// 
/// </summary>
public abstract class Interactor : MonoBehaviour
{
    private InputAction _interactAction;
    

    private void Start()
    {
        _interactAction = InputManager.Instance.GetInteractAction();
        _interactAction.Enable();
        
        _interactAction.performed += OnInteractKeyClicked;
    }



    private void OnInteractKeyClicked(InputAction.CallbackContext obj)
    {
            Interact();
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out IUIDisplayable ui))
        {
            ui.ShowInteractUI();
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out IUIDisplayable ui))
        {
            ui.HideInteractUI();
        }
    }

    protected abstract void Interact();






}


