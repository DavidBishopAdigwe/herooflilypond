using Enums;
using Singletons;
using Singletons.BaseManagers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MenuManager : MonoBehaviourSingleton<MenuManager>
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pausedMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject volumeMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject screenMenu;
    [SerializeField] private GameObject tutorialPrompt;
    [SerializeField] private GameObject escButton;
    [SerializeField] private GameObject pauseIcon;
    [SerializeField] public MenuState currentMenuState = MenuState.None;
    


    protected override void Awake()
    {
        base.Awake();

        InputReader.Instance.BackPerformed += OnBackKeyClicked;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode l)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            currentMenuState = MenuState.MainMenu;
            mainMenu.SetActive(true);
        }
    }

    public void PlayButton()
    {
        currentMenuState = MenuState.TutorialPrompt;
        tutorialPrompt.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void TutorialYesButton()
    {
        GameStarted();
        
        GameManager.Instance.EndTutorial();
        currentMenuState = MenuState.Playing;

        SceneManager.LoadScene("Main");
    }

    public void TutorialNoButton()
    {
        GameStarted();
        
        GameManager.Instance.StartTutorial();
        currentMenuState = MenuState.Playing;
        
        SceneManager.LoadScene("Main");
    }

    private void GameStarted()
    {
        pauseIcon.SetActive(true);
        escButton.SetActive(false);
        tutorialPrompt.SetActive(false);

    }


    public void OptionsButton()
    {
        currentMenuState = MenuState.Options;
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

        currentMenuState = MenuState.Options;
    }

    public void VolumeButton()
    {
        optionsMenu.SetActive(false);
        volumeMenu.SetActive(true);
        currentMenuState = MenuState.Volume;
    }

    public void ScreenButton()
    {
        optionsMenu.SetActive(false);
        screenMenu.SetActive(true);
        currentMenuState = MenuState.Screen;
    }

    public void CreditsButton()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
        currentMenuState = MenuState.Credits;
    }

    public void QuitButton()
    {
        Application.Quit();
    }


    public void ResumeButton()
    {
        Time.timeScale = 1;
        pausedMenu.SetActive(false);
        currentMenuState = MenuState.Playing;
    }

    public void MainMenuButton()
    {
        currentMenuState = MenuState.MainMenu;
        Time.timeScale = 1;
        pausedMenu.SetActive(false);
        SceneManager.LoadScene(0);
        mainMenu.SetActive(true);
    }

    public void EscButton()
    {
        GoBack();
    }

    public void PauseButton()
    {
        currentMenuState = MenuState.Paused;
        Time.timeScale = 0;
        pausedMenu.SetActive(true);
        pauseIcon.SetActive(false);
        escButton.SetActive(true);
    }

    private void OnBackKeyClicked(InputAction.CallbackContext obj)
    {
        GoBack();
    }
    

    private void GoBack()
    {
        switch (currentMenuState)
        {
            case MenuState.Playing:
                pausedMenu.SetActive(true);
                pauseIcon.SetActive(false);
                escButton.SetActive(true);
                Time.timeScale = 0;
                currentMenuState = MenuState.Paused;
                break;

            case MenuState.Paused:
                pausedMenu.SetActive(false);
                escButton.SetActive(false);
                pauseIcon.SetActive(true);
                Time.timeScale = 1;
                currentMenuState = MenuState.Playing;
                break;

            case MenuState.MainMenu:
                currentMenuState = MenuState.MainMenu;
                break;

            case MenuState.Options when Time.timeScale == 0:
                pausedMenu.SetActive(true);
                optionsMenu.SetActive(false);
                currentMenuState = MenuState.Paused;
                break;

            case MenuState.Options when Time.timeScale.Equals(1):
                optionsMenu.SetActive(false);
                mainMenu.SetActive(true);
                currentMenuState = MenuState.MainMenu;
                break;

            case MenuState.Volume:
                volumeMenu.SetActive(false);
                optionsMenu.SetActive(true);
                currentMenuState = MenuState.Options;
                break;

            case MenuState.Screen:
                screenMenu.SetActive(false);
                optionsMenu.SetActive(true);
                currentMenuState = MenuState.Options;
                break;

            case MenuState.Credits:
                creditsMenu.SetActive(false);
                mainMenu.SetActive(true);
                currentMenuState = MenuState.MainMenu;
                break;


            case MenuState.TutorialPrompt:
                tutorialPrompt.SetActive(false);
                mainMenu.SetActive(true);
                currentMenuState = MenuState.MainMenu;
                break;
        }
    }
}