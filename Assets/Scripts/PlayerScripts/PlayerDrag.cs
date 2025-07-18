
using System;
using Singletons;
using PlayerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Enums;
using PlayerScripts.PlayerInteractor;

public class PlayerDrag : CollisionInteractor
{
    [SerializeField] private float draggingSpeed;
    
    private FixedJoint2D _joint;
    private Collider2D _objectCollider;
    private bool _attached;
    private PlayerController _playerController;
    private PlayerItemTracker _playerItemTracker;
    private bool _hasRope;
    private DraggableObject _currentDraggableObject;
    private AudioClip _dragClip;

    public event Action TryDragWithoutRope;

    private void Awake()
    {
        _joint             = GetComponent<FixedJoint2D>();
        _playerController  = GetComponent<PlayerController>();
        _playerItemTracker = GetComponent<PlayerItemTracker>();
    }

    protected override void Start()
    {
        base.Start();
        DetachFromObject();
    }
    
    

    protected override void OnInteractKeyClicked(InputAction.CallbackContext obj)
    {
        if (!_playerItemTracker.PlayerHasRope() && _objectCollider != null)
        {
            MessageManager.Instance.ShowMessage("Locate a rope to drag boxes", MessageType.Error);
            TryDragWithoutRope?.Invoke();
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
        _joint.connectedBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        PlayAudio();
        
        _playerController.ChangeMovementSpeed(draggingSpeed);
        _attached = true;
    }

    private void PlayAudio()
    {
        if (_joint.connectedBody.gameObject.TryGetComponent(out DraggableObject draggable))
        {
            _currentDraggableObject = draggable;
            _currentDraggableObject.PlayAttachAnimation();
        } 
    }
    
    
    public void DetachFromObject()
    { 
        if (_currentDraggableObject) _currentDraggableObject.ResetToIdle();
        
        _joint.enabled = false;
        _playerController.ResetMovementSpeed();
        _attached = false;
        
        if (_joint.connectedBody)
        {
            _joint.connectedBody.bodyType = RigidbodyType2D.Kinematic;
            _joint.connectedBody.constraints = RigidbodyConstraints2D.FreezePosition;
            _joint.connectedBody = null;
        }

        
    }


    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DraggableBox"))
        {
            base.OnCollisionEnter2D(other);
            _objectCollider = other.collider;
        }
    }

    protected override void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DraggableBox"))
        {
            base.OnCollisionExit2D(other);
            _objectCollider = null;
        }
    }
    
    
    
    public bool IsPlayerAttached() => _attached;
    
    
}
