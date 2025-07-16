using System;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Base class for all scriptable objects which are also singletons
    /// </summary>

    public class ScriptableObjectSingleton<T> : ScriptableObject where T: ScriptableObject // Might swap to unity's variant? But i think this performs it's purpose
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
}
