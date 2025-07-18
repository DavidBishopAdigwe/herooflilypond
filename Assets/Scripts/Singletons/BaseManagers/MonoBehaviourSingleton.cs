using UnityEngine;

namespace Singletons.BaseManagers
{
    public abstract class MonoBehaviourSingleton<T>: MonoBehaviour where T: MonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad;
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
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
} 

