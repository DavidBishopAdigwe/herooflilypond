using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Enums;
using UnityEngine.AI;

namespace Managers
{
    public class GameManager: MonoBehaviour
    {
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [SerializeField] private int timeToEndGame = 1;
        private List<Enemy> _enemiesInScene = new();
        public static GameManager Instance { get; private set; }




        public void GameOver()
        {
            MessageMaster.Instance.ShowMessage("GAME OVER", MessageType.Error);
            Invoke("ReloadScene", 1);
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(0);
        }

        private void MM()
        {
            MenuManager.Instance.MainMenuButton();

        }

        public void WinGame()
        {
            MessageMaster.Instance.ShowMessage("CONGRATS, You Win");
            Invoke("ReloadScene", 1.5f);

        }

        public void LoadNewScene(int index)
        {
            SceneManager.LoadScene(index);
            _enemiesInScene.Clear();
        }

        public void LoadNewScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            _enemiesInScene.Clear();
        }
        

        private void OnApplicationQuit()
        {
            SceneManager.LoadScene(2);
        }
    }
}
