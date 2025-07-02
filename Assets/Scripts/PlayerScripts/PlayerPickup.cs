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

public class PlayerPickup : MonoBehaviour
{ 
    [SerializeField] private PlayerLightSourceController playerLightSourceController;
    [SerializeField] private Lamp lightSource;
    [SerializeField] private PlayerObjectHaver playerObjectHaver;
    [SerializeField] private Health playerHealth;
    private InputAction _interactAction;
    private PickableObject _bestObject;

    private readonly List<PickableObject> _objectsInRange = new ();

    private void Start()
    {
        
        _interactAction = InputManager.Instance.GetInteractAction();
        _interactAction.Enable();
        
        _interactAction.performed += OnInteractKeyClicked;
        lightSource.OnLightToggled += OnLightStateChanged;
        
    }



    private void OnInteractKeyClicked(InputAction.CallbackContext obj)
    {
        {
    
            float closestDistance = Mathf.Infinity;
            _bestObject = null;

            foreach (var pickableObject in (_objectsInRange))
            {
                float distance = Vector2.Distance(transform.position, pickableObject.transform.position);
                
                if ((distance > closestDistance)) continue;
                
                closestDistance = distance;
                _bestObject = pickableObject;
            }

            if (_bestObject == null || !_bestObject.TryGetComponent(out PickableObject interactable)
                                       || interactable.IsHidable() && !lightSource.IsLightOn()) 
                return;
            
            PickupObject(interactable);  
        }
    }


    private void OnLightStateChanged(bool isLightOn)
    {
        var hidableObjects = _objectsInRange.Where(p => p.IsHidable());
        
        foreach (var hidable in hidableObjects) 
        {
            if (!hidable.TryGetComponent(out IUIDisplayable ui)) continue;
            
            if (isLightOn) ui.ShowInteractUI();
            else           ui.HideInteractUI();
        }
    }
    

    private void OnDestroy()
    {
        lightSource.OnLightToggled -= OnLightStateChanged;
        
        _interactAction.performed -= OnInteractKeyClicked;
      
        _interactAction.Disable();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PickableObject pickableObject) && !_objectsInRange.Contains(pickableObject))
        {
            _objectsInRange.Add(pickableObject);
        }
        
        if (!pickableObject.TryGetComponent(out IUIDisplayable ui)) return;
        
        ui.HideInteractUI();
    
        if (pickableObject.IsHidable() && lightSource.IsLightOn())
        {
            ui.ShowInteractUI();
        }
        else
        {
            ui.ShowInteractUI();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IUIDisplayable ui) && other.TryGetComponent(out PickableObject pickableObject))
        {
            ui.HideInteractUI();
            _objectsInRange.Remove(pickableObject);
        }
    }

    private void PickupObject(PickableObject interactable)
    {
        interactable.Pickup();
        switch (interactable) 
        {
            case not null when interactable.CompareTag("Rope"):
                playerObjectHaver.PickedRope();
                break;
            case not null when interactable.CompareTag("Key"):
                playerObjectHaver.PickedKey();
                break;
            case not null when interactable.CompareTag("Lamp"):
            {
                break;
            }
            case PickableLampOil oil when playerObjectHaver.PlayerHasLamp():
                oil.AddOilToLamp(lightSource);
                break;
            case PickableHealthPotion hpPot:
                hpPot.AddHp(ref playerHealth);
                break;
        }
    }
    
}


