using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class MovementAreaManager : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    private List<MovementArea> _movementAreas = new List<MovementArea>();

    private void Awake()
    {
        _movementAreas.Clear();
        foreach (MovementArea area in FindObjectsByType<MovementArea>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            _movementAreas.Add(area);
        }
    }

    private void Start()
    {
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