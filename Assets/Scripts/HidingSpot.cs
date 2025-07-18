
    using System;
    using Interfaces;
    using UnityEngine;

    public class HidingSpot: UIDisplayableObject
    {
        
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

        
    }
