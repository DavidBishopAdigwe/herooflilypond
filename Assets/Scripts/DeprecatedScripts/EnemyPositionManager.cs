using System.Collections.Generic;
using DataPersistence.Data;
using Interfaces;
using UnityEngine;

namespace Managers
{
    public class EnemyPositionManager : MonoBehaviour
    {
        private List<EnemyAI> _enemies = new List<EnemyAI>();
        private List<Vector3> _enemyPositions = new List<Vector3>();
        public static EnemyPositionManager Instance { get; private set; }

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

        public void AddEnemy(EnemyAI enemy)
        {
            if (!_enemies.Contains(enemy))
            {
                _enemies.Add(enemy);
            }
        }

        public void GetEnemyPositions()
        {
            _enemyPositions.Clear();
            foreach (var enemy in _enemies)
            {
                _enemyPositions.Add(enemy.transform.position);
            }
        }

        public void ResetEnemyPositions()
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemyPositions == null) return;
                if (i < _enemyPositions.Count && i < _enemies.Count)
                {
                    _enemies[i].transform.position = _enemyPositions[i];
                }

                StartCoroutine(_enemies[i].ResetEnemies());
            }
        }


    }
}