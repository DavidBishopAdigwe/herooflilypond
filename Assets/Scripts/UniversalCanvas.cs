
using System;
using UnityEngine;

public class UniversalCanvas: MonoBehaviour
{
    
    public static UniversalCanvas Instance { get; private set; }
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
} 

