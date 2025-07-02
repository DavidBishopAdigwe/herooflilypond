using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Managers
{
    public class GameManager: MonoBehaviour
    {

        [SerializeField] private int timeToEndGame = 1;
        private List<Enemy> _enemiesInScene = new();
        public static GameManager Instance { get; private set; }
        

        public void StartGame()
        {
            LoadNewScene(0);
        }

        public void GameOver()
        {
            MessageManager.Instance.ShowMessage("Game Over");
            Invoke("StartGame", timeToEndGame);
        }

        public void WinGame()
        {
            MessageManager.Instance.ShowMessage("CONGRATS");
            Invoke("StartGame", timeToEndGame);
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

        public void FindEnemyToConverseWith(Enemy chasingEnemy)
        {
            var distance = Mathf.Infinity;
            Vector3 conversationPos = new Vector3();
            foreach (Enemy enemy in _enemiesInScene)
            {
                var enemyPosition = enemy.transform.position;
                var enemyDistance = Vector2.Distance(enemy.transform.position, enemyPosition);
                if (enemyDistance < distance)
                {
                    distance = enemyDistance;
                    conversationPos = enemyPosition;
                }
            }

            NavMeshAgent chasingEnemyAgent = chasingEnemy.GetComponent<NavMeshAgent>();
            chasingEnemyAgent.SetDestination(conversationPos);
        }
    }
}
