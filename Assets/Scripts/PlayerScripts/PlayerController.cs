using System;
using System.Numerics;
using Singletons;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;


public class PlayerController : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    
    [SerializeField] private float baseMovementSpeed;
    [SerializeField] private int playerLayer;
    [SerializeField] private AudioClip movementSound;
    [SerializeField] private AudioClip draggingMovementSound;
    [SerializeField] private GameObject[] unflipableObjects;
    
    private Rigidbody2D _rb; 
    private Animator _animator;
    private Vector2 _inputValue;
    private PlayerDrag _playerDrag;
    private AudioSource _audioSource;
    private bool _isMoving;
    private float _currentSpeed;

    public event Action PlayerMoved;

    private void Awake()
    {
        _rb            = GetComponent<Rigidbody2D>();
        _animator      = GetComponent<Animator>();
        _playerDrag    = GetComponent<PlayerDrag>();
        _audioSource = GetComponent<AudioSource>();
        _currentSpeed  = baseMovementSpeed;
    }
    private void Start()
    {
        InputReader.Instance.MovePerformed += OnMoveKeysClicked;
        InputReader.Instance.MoveCanceled += OnMoveKeysReleased;
        InputReader.Instance.UnsubscribeMove += UnsubscribeInputs;
        InputReader.Instance.SubscribeMove += SubscribeInputs;

    }
    
    
    

    private void OnMoveKeysClicked(InputAction.CallbackContext obj)
    {
        PlayerMoved?.Invoke();
        _inputValue = obj.ReadValue<Vector2>().normalized;
        _isMoving   = true;
        
        PlayAudio();
        _animator.SetBool(IsMoving, true);
        
        if (!Mathf.Approximately(Mathf.Sign(_inputValue.x), Mathf.Sign(transform.right.x)))
        {
            Flip();
        }
    }

    private void PlayAudio()
    {
        if (_audioSource.isPlaying) _audioSource.Stop();
        
        if (!_playerDrag.IsPlayerAttached())
        {
            _audioSource.clip = movementSound;
            _audioSource.Play();
        }
        else
        {
            _audioSource.clip = draggingMovementSound;
            _audioSource.Play();
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
        if (_playerDrag.IsPlayerAttached()) return;
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
        StopMovement();
    }

    private void StopMovement()
    {
        _audioSource.Stop();

        _isMoving = false;
        _animator.SetBool(IsMoving, false);
        _rb.linearVelocity = Vector2.zero;
    }

    public void SubscribeInputs()
    {
        InputReader.Instance.MovePerformed += OnMoveKeysClicked;
        InputReader.Instance.MoveCanceled += OnMoveKeysReleased;

    }
    

    private void OnDestroy()
    {
        UnsubscribeInputs();
        InputReader.Instance.UnsubscribeMove -= UnsubscribeInputs;
        InputReader.Instance.SubscribeMove -= SubscribeInputs;
    }



}
