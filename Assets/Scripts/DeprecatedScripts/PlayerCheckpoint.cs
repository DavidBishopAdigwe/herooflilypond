using System.Collections;
using DataPersistence;
using DataPersistence.Data;
using Interfaces;
using Managers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCheckpoint: MonoBehaviour, IDataPersistence
    {
        [SerializeField] private GameObject blackBox; 
        [SerializeField]  private CheckPoint spawnPoint;
        private Vector3 _checkPointPosition;
        private CheckPoint _currentCheckpoint;
        private PlayerHide _playerHide;

        private void Awake()
        {
            _playerHide = GetComponent<PlayerHide>();
            
            _currentCheckpoint = spawnPoint;
            
            _checkPointPosition = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        }

        

        private void ResetsOnSpawn()
        {
            if (_playerHide == null)
            {
                return;
            }
            _playerHide.ResetHideEffect();
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out CheckPoint checkPoint)) return;
            
            _currentCheckpoint = checkPoint;
            _checkPointPosition = collision.gameObject.transform.position;
            DataPersistenceManager.Instance.SaveGame();
        }
        
        public void MovePlayerToCheckPoint()
        {
            blackBox.SetActive(true);
            
            if (_checkPointPosition != Vector3.zero || _currentCheckpoint != null)
            {
                gameObject.transform.position = _checkPointPosition;
                ResetsOnSpawn(); 
            }
            
            StartCoroutine(ResetTime());
            Time.timeScale = 0;
        }

        private IEnumerator ResetTime()
        {
            yield return new WaitForSecondsRealtime(1);
            StartTime();
            
        }

        private void StartTime()
        {
            
            Time.timeScale = 1;
            blackBox.SetActive(false);
        }

        public void LoadData(GameData data)
        {
            _checkPointPosition = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            
            if (_checkPointPosition == Vector3.zero)
            {
                _checkPointPosition = spawnPoint.transform.position;
            }
            MovePlayerToCheckPoint();
        }

        public void SaveData(ref GameData data)
        {
            data.playerPosition = new float[] { _checkPointPosition.x, _checkPointPosition.y, _checkPointPosition.z };
        }
    }
