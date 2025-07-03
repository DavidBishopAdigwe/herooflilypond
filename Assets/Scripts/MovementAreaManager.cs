using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/MovementAreaManager", fileName = "MovementAreaManager")]
public class MovementAreaManager : ScriptableObjectSingleton<MovementAreaManager>
{
    [SerializeField] private Enemy enemyPrefab;
    private List<MovementArea> _movementAreas = new List<MovementArea>();
    

    private void OnEnable()
    {
        SceneManager.sceneLoaded += FindMovementAreasInScene;
    }

    private void FindMovementAreasInScene(Scene scene, LoadSceneMode mode)
    {
        _movementAreas.Clear();
        foreach (MovementArea area in FindObjectsByType<MovementArea>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            _movementAreas.Add(area);
        }
        
        if (_movementAreas.Count <= 0)
        {
            Console.WriteLine("zero");
            return;
        }
        SpawnEnemies();
    }


    private void SpawnEnemies()
    {
        foreach (var area in _movementAreas)
        {
            var enemyClone = Instantiate(enemyPrefab);
            enemyClone.Setup(area);
        }
    }
    
}