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
public class PlayerPickup : Interactor
{
    
    [FormerlySerializedAs("_healthPickupSound")] [SerializeField] private AudioClip _pickupSound;
    
    private PlayerController _player;
    private LightSource _lightSource;
    private PlayerItemTracker _playerItemTracker; 
    private Health _playerHealth;
    private PickableItem _bestItem;
    private bool _lightOn;
    private readonly List<PickableItem> _objectsInRange = new ();
    private AudioSource _audioSource;

    public event Action PickedHealthPotion; // Tutorial event

    private void Awake()
    {
        _player = GetComponentInParent<PlayerController>();
        _audioSource = GetComponentInParent<AudioSource>();
        _playerItemTracker = _player.GetComponent<PlayerItemTracker>();
        _playerHealth = _player.GetComponent<Health>();
        _lightSource = _player.GetComponentInChildren<LightSource>();
    }

    protected override void Start()
    {
        base.Start();
        _lightSource.OnLightToggled += OnLightStateChanged;
    }


    protected override void OnInteractKeyClicked(InputAction.CallbackContext obj)
    {
        {
    
            float closestDistance = Mathf.Infinity;
            _bestItem = null;

            foreach (var pickableObject in (_objectsInRange))
            {
                float distance = Vector2.Distance(transform.position, pickableObject.transform.position);
                
                if ((distance > closestDistance)) continue;
                
                closestDistance = distance;
                _bestItem = pickableObject;
            }

            if (_bestItem == null || !_bestItem.TryGetComponent(out PickableItem interactable)
                                     || interactable.IsHidable() && !_lightOn) return;
            
            PickupObject(interactable);  
        }
    }
    
    


    private void OnLightStateChanged(bool isLightOn)
    {
        _lightOn = isLightOn;
        var hideableObjects = _objectsInRange.Where(p => p.IsHidable());
        
        foreach (var hideable in hideableObjects) 
        {
            if (!hideable.TryGetComponent(out UIDisplayableObject ui)) continue;
            
            if (isLightOn) ui.ShowInteractUI();
            else           ui.HideInteractUI();
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        _lightSource.OnLightToggled -= OnLightStateChanged;
        
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PickableItem pickableObject) && !_objectsInRange.Contains(pickableObject))
        {
            _objectsInRange.Add(pickableObject);
        }

        if (pickableObject && pickableObject.TryGetComponent(out UIDisplayableObject ui))
        {
    
            if (pickableObject.IsHidable() && !_lightOn)
            {
                ui.HideInteractUI();
            }
            else
            {
                ui.ShowInteractUI();
            }
        }
        

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out UIDisplayableObject ui) && other.TryGetComponent(out PickableItem pickableObject))
        {
            ui.HideInteractUI();
            _objectsInRange.Remove(pickableObject);
        }
    }

    private void PickupObject(PickableItem interactable)
    {
        interactable.Pickup();
        _audioSource.PlayOneShot(_pickupSound);
        switch (interactable) 
        {
            case not null when interactable.CompareTag("Rope"):
                _playerItemTracker.PickedRope();
                break;
            case not null when interactable.CompareTag("Lamp"):
            {
                _playerItemTracker.PickedLamp();
                _lightSource.PickedUpLamp();
                break;
            }
            case PickableOil oil when _playerItemTracker.PlayerHasLamp():
                oil.AddOilToLamp(_lightSource);
                break;
            case PickableHealthPotion hpPot:
                PickedHealthPotion?.Invoke();
                hpPot.AddHp(_playerHealth);
                break;
        }
    }
    
    
    
}


