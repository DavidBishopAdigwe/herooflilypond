using System;
using Interfaces;
using Managers;
using PlayerScripts;
using UnityEngine;

namespace PickableItems
{
    // Base class for all pickable items.
    public class PickableObject : MonoBehaviour, IUIDisplayable
    { 
        [SerializeField] private string objectName;
        [SerializeField] private GameObject interactionUI;

        protected bool Hidable;
        protected PlayerObjectHaver PlayerObjHaver;

        protected virtual void Start()
        {
            PlayerObjHaver = AssignmentManager.Instance.GetPlayerObjectHaver();
        }

        public bool IsHidable()
        {
            return Hidable;
        }
        
        public virtual void Pickup()
        {
            MessageManager.Instance.ShowMessage("Picked up " + objectName);
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
