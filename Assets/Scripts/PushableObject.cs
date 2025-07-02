using Interfaces;
using UnityEngine;

    public class PushableObject : MonoBehaviour, IUIDisplayable
    {
        
        [SerializeField] private GameObject interactionUI;
        
        public void ShowInteractUI()
        {
            interactionUI.SetActive(true);
        }

        public void HideInteractUI()
        {
            interactionUI.SetActive(false);
        }
    }