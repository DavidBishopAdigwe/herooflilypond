using Interfaces;
using UnityEngine;

    public class DraggableObject : UIDisplayableObject
    {
        
        
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayAttachAnimation()
        {
            _animator.SetTrigger("Attach");
        }

        public void ResetToIdle()
        {
            _animator.SetTrigger("Detach");
        }
        
    }