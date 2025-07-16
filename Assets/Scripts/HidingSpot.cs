
    using System;
    using Interfaces;
    using UnityEngine;

    public class HidingSpot: MonoBehaviour, IUIDisplayable
    {
        
        [SerializeField] private GameObject interactionUI;
        private bool _playerCannotInteract;
        private Animator _animator;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            HideInteractUI();
        }

        public void HideAnimation()
        {
            _animator.SetTrigger("Hiding");
        }

        public void ShowInteractUI()
        {
            interactionUI.SetActive(true);
        }

        public void HideInteractUI()
        {
            interactionUI.SetActive(false);
        }
    }
