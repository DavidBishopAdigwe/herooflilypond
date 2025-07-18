using UnityEngine;

public class UIDisplayableObject: MonoBehaviour
{
        
    [SerializeField] private GameObject interactionUI;
    
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