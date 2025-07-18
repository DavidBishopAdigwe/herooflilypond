
using System.Collections.Generic;
using UnityEngine;

public class SoundManager: MonoBehaviour
{

    [SerializeField] private int maxAudioSources;
    private List<AudioSource> _activeSources;
    private List<AudioSource> _inactiveSources;
    
    private void CreatePool()
    {
        var pool = new GameObject();
        pool.AddComponent<AudioSource>();
    }
    
} 

