using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIScreenManager: MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pausedMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject volumeMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject screenMenu;
    [SerializeField] private GameObject quitPrompt;
    [SerializeField] public GameState CurrentGameState = GameState.None;
    
	private InputAction _pauseAction;
    private bool isPaused;
    
    public static UIScreenManager Instance { get; private set; }


    private void Awake()
    {
        _pauseAction = InputManager.Instance.GetPauseAction();
       
       _pauseAction.Enable();

       _pauseAction.performed += OnPauseKeyClicked;

       if (SceneManager.GetActiveScene().name == "Main")
       {
           CurrentGameState = GameState.IsPlaying;
           mainMenu.SetActive(false);
       }
    }
    
    public void PlayButton()
    {
        CurrentGameState = GameState.IsPlaying;
        mainMenu.SetActive(false);
        SceneManager.LoadScene("Main");
    }

    public void OptionsButton()
    {
        CurrentGameState = GameState.Options;
        if (Time.timeScale == 0)
        {
            mainMenu.SetActive(false);
            pausedMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }
        else
        {
            pausedMenu.SetActive(false);
            mainMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }
        CurrentGameState = GameState.Options;
    }

    public void VolumeButton()
    {
        optionsMenu.SetActive(false);
        volumeMenu.SetActive(true);
        CurrentGameState = GameState.Volume;
    }

    public void ScreenButton()
    {
        optionsMenu.SetActive(false);
        screenMenu.SetActive(true);
        CurrentGameState = GameState.Screen;
    }

    public void CreditsButton()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
        CurrentGameState = GameState.Credits;
    }

    public void QuitButton()
    {
        quitPrompt.SetActive(true);
        CurrentGameState = GameState.QuitPrompt;
    }


    public void ResumeButton()
    {
        Time.timeScale = 1;
        pausedMenu.SetActive(false);
        CurrentGameState = GameState.IsPlaying;
    }

    public void MainMenuButton()
    { 
        CurrentGameState = GameState.MainMenu;
        Time.timeScale = 1;
        pausedMenu.SetActive(false);
        SceneManager.LoadScene(0);
        mainMenu.SetActive(true);
    }

    private void OnPauseKeyClicked(InputAction.CallbackContext obj)
    {
        switch (CurrentGameState)
        {
            case GameState.IsPlaying:
                pausedMenu.SetActive(true);
                Time.timeScale = 0;
                CurrentGameState = GameState.IsPaused;
                break;
            
            case GameState.IsPaused:
                pausedMenu.SetActive(false);
                Time.timeScale = 1;
                CurrentGameState = GameState.IsPlaying;
                break;
            
            case GameState.MainMenu:
                CurrentGameState = GameState.MainMenu;
                break;
            
            case GameState.Options when Time.timeScale == 0:
                pausedMenu.SetActive(true);
                optionsMenu.SetActive(false);
                CurrentGameState = GameState.IsPaused;
                break;
            
            case GameState.Options when Time.timeScale.Equals(1):
                optionsMenu.SetActive(false);
                mainMenu.SetActive(true);
                CurrentGameState = GameState.MainMenu;
                break;
            
            case GameState.Volume:
                volumeMenu.SetActive(false);
                optionsMenu.SetActive(true);
                CurrentGameState = GameState.Options;
                break;
            
            case GameState.Screen:
                screenMenu.SetActive(false);
                optionsMenu.SetActive(true);
                CurrentGameState = GameState.Options;
                break;
            
            case GameState.Credits:
                creditsMenu.SetActive(false);
                mainMenu.SetActive(true);
                CurrentGameState = GameState.MainMenu;
                break;
            
            case GameState.QuitPrompt:
                quitPrompt.SetActive(false);
                CurrentGameState = GameState.Options;
                break; 
                
        }
    }

    public enum GameState
    {
        None,
        IsPlaying,
        IsPaused,
        Credits,
        MainMenu,
        Options,
        Volume,
        Screen,
        QuitPrompt
    }

    void OnPauseKeyClicked()
    {
       
    }
} 

