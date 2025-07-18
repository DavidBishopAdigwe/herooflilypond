using System;
using UnityEngine;

    public class HidingSpot: UIDisplayableObject
    {
        
        private bool _playerCannotInteract;
        private Animator _animator;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        

        public void HideAnimation()
        {
            _animator.SetTrigger("Hiding");
        }
        
    }
