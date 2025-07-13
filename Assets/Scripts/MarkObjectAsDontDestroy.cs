
using UnityEngine;

public class MarkObjectAsDontDestroy: MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
} 

