
using System;
using Interfaces;
using Managers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerDrag : MonoBehaviour
{
    [SerializeField] private float draggingSpeed;
    private FixedJoint2D _joint;
    private Collider2D _objectCollider;
    private InputAction _dragAction;
    private bool _attached;
    private PlayerController _playerController;
    private PlayerItemTracker _playerItemTracker;
    private bool _hasRope;

    private void Awake()
    {
        _joint = GetComponent<FixedJoint2D>();
        _playerController = GetComponent<PlayerController>();
        _playerItemTracker = GetComponent<PlayerItemTracker>();
    }

    private void Start()
    {
        _dragAction = InputManager.Instance.GetDragAction();
        SubscribeInputs(); 
        DetachFromObject();
    }
    

    private void OnDragKeyClicked(InputAction.CallbackContext obj)
    {
        if (!_playerItemTracker.PlayerHasRope())
        {
            Messages.Instance.DisplayMessage("Locate a rope to drag boxes", 2);
            return;
        }
        if (_objectCollider != null && !_attached)
        {
            AttachToObject();
        }
        else if (_attached)
        {
            DetachFromObject();
        }
    }

    private void AttachToObject()
    {
        _joint.enabled = true;
        _joint.connectedBody = _objectCollider.GetComponent<Rigidbody2D>();
        _joint.connectedBody.bodyType = RigidbodyType2D.Dynamic;
        _joint.connectedBody.constraints = RigidbodyConstraints2D.None;
        _playerController.ChangeMovementSpeed(draggingSpeed);
        _attached = true;
    }
    
    
    public void DetachFromObject()
    {
        _joint.enabled = false;
        _playerController.ResetMovementSpeed();
        _attached = false;
        if (_joint.connectedBody == null) return;
        _joint.connectedBody.bodyType = RigidbodyType2D.Kinematic;
        _joint.connectedBody.constraints = RigidbodyConstraints2D.FreezePosition;
        _joint.connectedBody = null;
        
    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out DraggableObject pushableObject) && other.gameObject.TryGetComponent(out IUIDisplayable ui))
        {
            _objectCollider = other.collider;
            ui.ShowInteractUI();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out DraggableObject pushableObject) && other.gameObject.TryGetComponent(out IUIDisplayable ui))
        {
            _objectCollider = null;
            ui.HideInteractUI();
        }
    }
    
    
    
    public bool IsPlayerConnected() => _attached;
    
    private void OnDestroy()
    {
        UnsubscribeInputs();
    }

    public void SubscribeInputs()
    {
        _dragAction.Enable();

        _dragAction.performed += OnDragKeyClicked;
    }

    public void UnsubscribeInputs()
    {
        _dragAction.performed -= OnDragKeyClicked;
        _dragAction.Disable();
    }
}
