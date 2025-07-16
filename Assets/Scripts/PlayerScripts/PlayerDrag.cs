
using System;
using Interfaces;
using Managers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Enums;
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
    private DraggableObject _currentDraggableObject;

    private void Awake()
    {
        _joint = GetComponent<FixedJoint2D>();
        _playerController = GetComponent<PlayerController>();
        _playerItemTracker = GetComponent<PlayerItemTracker>();
    }

    private void Start()
    {
        _dragAction = InputManager.Instance.GetInteractAction();
        SubscribeInputs(); 
        DetachFromObject();
    }
    

    private void OnDragKeyClicked(InputAction.CallbackContext obj)
    {
        if (!_playerItemTracker.PlayerHasRope() && _objectCollider != null)
        {
            MessageMaster.Instance.ShowMessage("Locate a rope to drag boxes", MessageType.Error);
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

        if (_joint.connectedBody.gameObject.TryGetComponent(out DraggableObject draggable))
        {
            _currentDraggableObject = draggable;
            _currentDraggableObject.PlayAttachAnimation();

        }
        
        _playerController.ChangeMovementSpeed(draggingSpeed);
        _attached = true;
    }
    
    
    public void DetachFromObject()
    { 
        if (_currentDraggableObject) _currentDraggableObject.ResetToIdle();
        
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
        if (other.gameObject.TryGetComponent(out DraggableObject draggable) && other.gameObject.TryGetComponent(out IUIDisplayable ui))
        {
            _objectCollider = other.collider;
            ui.ShowInteractUI();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DraggableBox") && other.gameObject.TryGetComponent(out IUIDisplayable ui))
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
