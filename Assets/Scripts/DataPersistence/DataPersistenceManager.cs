using System;
using System.Collections.Generic;
using System.Linq;
using DataPersistence.Data;
using Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DataPersistence
{
    [CreateAssetMenu(menuName = "ScriptableObjects/DataManager", fileName = "DataManager")]
    public class DataPersistenceManager: ScriptableObjectSingleton<DataPersistenceManager>
    {
        [Header("File Storage Config")] [SerializeField]
        private string fileName;
        
        public GameData gameData;
        
        private FileDataHandler _dataHandler;
        
        private List<IDataPersistence> _dataPersistenceObjects = new ();

        private void OnEnable()
        { }

        void Test()
        {
            if (SceneManager.sceneCountInBuildSettings != 0)
            {
                return;
            }
            SceneManager.sceneUnloaded += QuitGame;
            SaveGame();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= StartGame;
            SceneManager.sceneUnloaded -= QuitGame;
        }

        private void StartGame(Scene scene, LoadSceneMode mode)
        {
            _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            _dataPersistenceObjects = FindAllDataPersistenceObjects(); 
            LoadGame();
            Test();
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistence> dataPersistenceObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDataPersistence>();
            
            return new List<IDataPersistence>(dataPersistenceObjects);
        }

        public void NewGame()
        {
            gameData = new GameData();
        }

        public void LoadGame()
        {
            if (_dataHandler == null) // data handler nulls for some reason???
            {
                _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            }
            Debug.Log("Loaded");
            gameData = _dataHandler.Load();
            
            if (gameData == null)
            {
                NewGame();
            }
            
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                if (dataPersistenceObject == null) return;
                dataPersistenceObject.LoadData(gameData);
            }

        }

        public void SaveGame()
        {
            Debug.Log("Saved Game");
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                dataPersistenceObject.SaveData(ref gameData);
            }

            if (_dataHandler == null) // data handler nulls for some reason???
            {
                _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            }

            if (gameData == null)
            {
                gameData = new GameData();
            }
            _dataHandler.Save(gameData);
        }
        
        private void QuitGame(Scene scene)
        {
            SaveGame();
        }
        
    }
}
