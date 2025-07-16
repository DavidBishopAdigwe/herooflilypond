using System;
using Interfaces;
using Managers;
using PlayerScripts;
using UnityEngine;
using Enums;

namespace PickableItems
{
    /// <summary>
    /// Base class for all pickable items
    /// </summary>
    public class PickableItem : MonoBehaviour, IUIDisplayable
    { 
        [SerializeField] private string objectName;
        [SerializeField] private GameObject interactionUI;
        [SerializeField] private Material hideableMaterial;
        [SerializeField] private bool hideable;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            if (hideable)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
                _spriteRenderer.material = hideableMaterial;
            }
        }

        public bool IsHidable()
        {
            return hideable;
        }
        
        public virtual void Pickup()
        {
            MessageMaster.Instance.ShowMessage($"Picked up: {objectName}", MessageType.Success);
            Destroy(gameObject);
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
}
