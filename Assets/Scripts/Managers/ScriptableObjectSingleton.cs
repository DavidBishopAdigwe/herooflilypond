using System;
using UnityEngine;

// Base class for all singletons that are also scriptable objects
public class ScriptableObjectSingleton<T> : ScriptableObject where T: ScriptableObject
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T[] singletons = Resources.LoadAll<T> ("");
                
                if (singletons == null || singletons.Length < 1)
                {
                    throw new Exception($"No object of type {typeof(T)} found in Resources/");
                }
                if (singletons.Length > 1)
                {
                    Debug.LogWarning($"More than one object of type {typeof(T)} found in Resources/");
                }
                
                _instance = singletons[0];
            }
            
            return _instance;
        }
    }
    
    
}
