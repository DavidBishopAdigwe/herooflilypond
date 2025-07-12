using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
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
            SceneManager.LoadScene(0);
        }

        public void WinGame()
        {
            MessageManager.Instance.ShowMessage("CONGRATS");
           // Invoke("StartGame", timeToEndGame);
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

        public void AddEnemy(Enemy enemy)
        {
            _enemiesInScene.Add(enemy);
        }

        private void OnApplicationQuit()
        {
            SceneManager.LoadScene(2);
        }
    }
}
