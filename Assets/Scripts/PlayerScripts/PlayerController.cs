using System.Numerics;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;


public class PlayerController : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    
    [SerializeField] private float baseMovementSpeed;
    [SerializeField] private int playerLayer;
    [SerializeField] private GameObject[] unflipableObjects;
    
    private Rigidbody2D _rb; 
    private Animator _animator;
    private InputAction _playerMoveActions;
    private Vector2 _inputValue;
    private Vector2 _pickupDirection;
    private PlayerDrag _playerDrag;
    private bool _isMoving;
    private float _currentSpeed;

    private void Awake()
    {
        _rb           = GetComponent<Rigidbody2D>();
        _animator     = GetComponent<Animator>();
        _playerDrag   = GetComponent<PlayerDrag>();
        _currentSpeed = baseMovementSpeed;
    }
    private void Start()
    {
        InputReader.Instance.MovePerformed += OnMoveKeysClicked;
        InputReader.Instance.MoveCanceled += OnMoveKeysReleased;
    }
    
    

    private void OnMoveKeysClicked(InputAction.CallbackContext obj)
    {
        _inputValue = obj.ReadValue<Vector2>();
        _inputValue = Vector2.ClampMagnitude(_inputValue, 1);
        _isMoving   = true;
        _animator.SetBool(IsMoving, true);
        
        if (!Mathf.Approximately(Mathf.Sign(_inputValue.x), Mathf.Sign(transform.right.x)))
        {
            Flip();
        }
    }
    private void OnMoveKeysReleased(InputAction.CallbackContext obj)
    {
        StopMovement();
    }
    
    private void FixedUpdate()
    {
        if (!_isMoving) return;
        _rb.linearVelocity = _inputValue * _currentSpeed;
    }
    
    private void Flip()
    {
        if (_playerDrag.IsPlayerConnected()) return;
        transform.right = -transform.right;
        foreach (GameObject unflipable in unflipableObjects)
        {
            unflipable.transform.right = -unflipable.transform.right;
        }
    }
    
    public void ChangeMovementSpeed(float speed)
    {
        _currentSpeed = speed;
    }

    public void ResetMovementSpeed()
    {
        _rb.linearVelocity = Vector2.zero;
        _currentSpeed = baseMovementSpeed;
    }

    public void UnsubscribeInputs()
    {
        
        InputReader.Instance.MovePerformed -= OnMoveKeysClicked;
        InputReader.Instance.MoveCanceled -= OnMoveKeysReleased;
        /*_playerMoveActions.performed -= OnMoveKeysClicked;
        StopMovement();
        _playerMoveActions.canceled -= OnMoveKeysReleased;
        
        _playerMoveActions.Disable();*/
    }

    private void StopMovement()
    {
        _isMoving = false;
        _animator.SetBool(IsMoving, false);
        _rb.linearVelocity = Vector2.zero;
    }

    public void SubscribeInputs()
    {
        _playerMoveActions.Enable();
        
        _playerMoveActions.performed += OnMoveKeysClicked;
        _playerMoveActions.canceled += OnMoveKeysReleased;
    }

    private void OnDestroy()
    {
        UnsubscribeInputs();
    }



}
