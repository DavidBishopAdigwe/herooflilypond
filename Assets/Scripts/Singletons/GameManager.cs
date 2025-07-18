using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Enums;
using Singletons.BaseManagers;

namespace Singletons
{
    public class GameManager: MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private int timeToEndGame = 1;

        private bool _inTutorial;
        private bool _startVideoPlayed;


        private void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void StartTutorial()
        {
            _inTutorial = true;
        }

        public void FinishStartVideo()
        {
            _startVideoPlayed = true;
        }

        public bool StartVideoHasBeenPlayed() => _startVideoPlayed;

        public void EndTutorial()
        {
            _inTutorial = false;
        }
        
        public bool PlayerInTutorial() => _inTutorial;
        

        public void WinGame()
        {
            MessageManager.Instance.ShowMessage("Congratulations. The first ingredient for the cure has been found", MessageType.Success);
            
            MessageManager.Instance.ShowMessage("You are one step closer to becoming our Hero of Lilypond.", MessageType.Success);
            Invoke("LoadMainMenu", timeToEndGame);

        }
        
        public void GameOver()
        {
            MessageManager.Instance.ShowMessage("GAME OVER", MessageType.Error);
            Invoke("LoadMainMenu", timeToEndGame);
        }

         
    }
}
