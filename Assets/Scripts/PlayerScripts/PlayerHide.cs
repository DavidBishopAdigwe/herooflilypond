using System;
using System.Collections;
using Enums;
using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PlayerScripts
{
    public class PlayerHide : CollisionInteractor
    {
        
        [SerializeField] private float hideCooldown = 1f; 
        [SerializeField, Tooltip("Takes the player 1/HideSpeed seconds to hide")] private float hideSpeed = 2f;
        [SerializeField] private UnityEvent onHide;
        [SerializeField] private HideState currentState = HideState.NotHiding;
        
        private PlayerController _playerController;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;
    
        private Collider2D _currentHidingSpot;
        private string _originalSortingLayer;
        private Vector3 _hideStartPosition;
        private Coroutine _hideCoroutine;
        private static bool _playerIsHidden;


        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playerController = GetComponent<PlayerController>();
        }



        protected override void OnInteractKeyClicked(InputAction.CallbackContext obj)
        {

            if (currentState == HideState.CannotHide || IsHidingInProgress()) return;
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
            }
            _hideCoroutine = StartCoroutine(currentState == HideState.NotHiding ? Hide() : Unhide());
        }

        private IEnumerator Hide()
        {
            if (_currentHidingSpot == null || currentState != HideState.NotHiding) yield break;
            
            StartHide();
               
            var color = _spriteRenderer.color;
            color.a   = 1;
            while (color.a > 0)
            {
                color.a               = Mathf.Max(0, color.a - hideSpeed * Time.deltaTime);
                _spriteRenderer.color = color;
                transform.position = Vector2.Lerp(transform.position, _currentHidingSpot.bounds.center, hideSpeed * Time.deltaTime);
                yield return null;
            }
                
            CompleteHide();

        }



        private IEnumerator Unhide()
        {
            if (currentState != HideState.Hidden) yield break;
            
            StartUnhide();
        
            var color = _spriteRenderer.color;
            color.a = 0;
            while (color.a < 1)
            {
                color.a               = Mathf.Min(1, color.a + hideSpeed * Time.deltaTime);
                _spriteRenderer.color = color;
                transform.position = Vector2.Lerp(transform.position, _hideStartPosition, hideSpeed * Time.deltaTime);
                yield return null;
            }
            CompleteUnhide();
            yield return new WaitForSeconds(hideCooldown);
            currentState = HideState.NotHiding;
        }
        
        private void StartHide()
        {
            InputReader.Instance.UnsubscribeMoveAction();
            currentState          = HideState.Hiding;
            onHide.Invoke();
            _hideStartPosition    = transform.position;
            _originalSortingLayer = _spriteRenderer.sortingLayerName;
            Physics2D.IgnoreCollision(_collider, _currentHidingSpot, true);
            if (_currentHidingSpot.TryGetComponent(out HidingSpot hidingSpot))
            {
                hidingSpot.HideAnimation();
            }
        }

        private void CompleteHide()
        {
            transform.position               = _currentHidingSpot.bounds.center;
            _spriteRenderer.enabled          = false;
            _collider.enabled                = false;
            _spriteRenderer.sortingLayerName = "Hiding";
            currentState                     = HideState.Hidden;
            _hideCoroutine                   = null;
            
            if (_currentHidingSpot.TryGetComponent(out HidingSpot hidingSpot))
            {
                hidingSpot.HideAnimation();
            }
        }

        private void StartUnhide()
        {
            currentState = HideState.Unhiding;
            _spriteRenderer.enabled = true;
            _collider.enabled = true;
            if (_currentHidingSpot.TryGetComponent(out HidingSpot hidingSpot))
            {
                hidingSpot.HideAnimation();
            }
        }

        private void CompleteUnhide()
        {
            _spriteRenderer.sortingLayerName = _originalSortingLayer;
            Physics2D.IgnoreCollision(_collider, _currentHidingSpot, false);
            transform.position = _hideStartPosition;
            
            InputReader.Instance.SubscribeMoveAction();
            _currentHidingSpot = null;
            _hideCoroutine = null;
            currentState = HideState.CannotHide;
            
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("HidingSpot")) return;
            base.OnCollisionEnter2D(other);
            _currentHidingSpot = other.collider;
        }

        protected override void OnCollisionExit2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("HidingSpot")) return;
            base.OnCollisionExit2D(other);
            if (currentState == HideState.NotHiding || currentState == HideState.CannotHide )
            {
                _currentHidingSpot = null;
                
            }
        }

        public void ResetHideEffect()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }

            if (IsHidingInProgress())
            {
                transform.position = _hideStartPosition;
            }
        
            if (!IsHidingInProgress())  return;

            Physics2D.IgnoreCollision(_collider, _currentHidingSpot, false);
            
            _spriteRenderer.enabled          = true;
            _collider.enabled                = true;
            _currentHidingSpot               = null;
            _spriteRenderer.sortingLayerName = _originalSortingLayer;
            var color                        = _spriteRenderer.color;
            color.a                          = 1;
            _spriteRenderer.color            = color;
            
            InputReader.Instance.SubscribeMoveAction();

            
            currentState = HideState.NotHiding;
        }

        public void StopPlayerHiding()
        {
            ResetHideEffect();
            transform.position = _hideStartPosition;
            currentState = HideState.CannotHide;
            Invoke("PlayerCanHide", 3);
        }

        private void PlayerCanHide()
        {
            currentState = HideState.NotHiding;
        }
        public bool IsHidingInProgress() => currentState == HideState.Hiding || currentState == HideState.Unhiding;
        public bool IsHidden()               => currentState == HideState.Hidden;


        


    }
}