using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    private InputAction _pauseAction;
    
    private void Start()
    {
        
        _pauseAction = InputManager.Instance.GetPauseAction();
        _pauseAction.Enable();
        
        _pauseAction.performed += OnPauseClicked;
        
        pauseMenu.SetActive(false);
    }

    private void OnPauseClicked(InputAction.CallbackContext obj)
    {
        if (!pauseMenu.activeSelf && optionsMenu.activeSelf) // may swap to storing previous active object to go back, but for now this works
        {
            optionsMenu.SetActive(false);
            pauseMenu.SetActive(true); 
            return;
        }
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        Time.timeScale = Mathf.Approximately(Time.timeScale, 1) ? 0 : 1;
    }

    private void OnDestroy()
    {
        _pauseAction.performed -= OnPauseClicked;
        
        _pauseAction.Disable();
    }
    
}
