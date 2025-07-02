using System;
using System.Collections.Generic;
using System.Linq;
using DataPersistence.Data;
using Interfaces;
using UnityEngine;

namespace DataPersistence
{
    public class DataPersistenceManager: MonoBehaviour
    {
        [Header("File Storage Config")] [SerializeField]
        private string fileName;
        
        public GameData gameData;
        
        private FileDataHandler _dataHandler;
        
        private List<IDataPersistence> _dataPersistenceObjects = new ();
        public static DataPersistenceManager Instance { get; private set; }

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

        private void Start()
        {
            _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            _dataPersistenceObjects = FindAllDataPersistenceObjects(); 
            LoadGame();
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
            gameData = _dataHandler.Load();
            
            if (gameData == null)
            {
                NewGame();
            }
            
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                if (dataPersistenceObject == null)
                {
                    return;
                }
                dataPersistenceObject.LoadData(gameData);
            }

        }

        public void SaveGame()
        {
            foreach (var dataPersistenceObject in _dataPersistenceObjects)
            {
                dataPersistenceObject.SaveData(ref gameData);
            }
            
            _dataHandler.Save(gameData);
        }
        
        private void OnApplicationQuit()
        {
            SaveGame();
        }
        
    }
}
