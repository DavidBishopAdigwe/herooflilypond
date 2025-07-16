
using System;
using System.Security.Cryptography;
using UnityEngine;

public class GameObjectSingleton<T>: MonoBehaviour where T: MonoBehaviour
{
    [SerializeField] private bool dontDestroyOnLoad;
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = gameObject.GetComponent<T>();
        }
        else
        {
            Destroy(gameObject);
        }

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
} 

