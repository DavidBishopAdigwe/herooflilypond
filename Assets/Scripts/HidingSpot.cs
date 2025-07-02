
    using System;
    using Interfaces;
    using UnityEngine;

    public class HidingSpot: MonoBehaviour, IUIDisplayable
    {
        
        [SerializeField] private GameObject interactionUI;
        private bool _playerCannotInteract;

        private void Start()
        {
            HideInteractUI();
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
