
using UnityEngine;
using Enums;
using Singletons;

namespace PickableItems
{
    /// <summary>
    /// Base class for all pickable items
    /// </summary>
    public class PickableItem : UIDisplayableObject
    { 
        [SerializeField] private string objectName;
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
            MessageManager.Instance.ShowMessage($"Picked up: {objectName}", MessageType.Success);
            Destroy(gameObject);
        }

 
        
    }
}
