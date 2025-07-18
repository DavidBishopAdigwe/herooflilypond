
using Singletons.BaseManagers;
using UnityEngine;

public class BackgroundMusic : MonoBehaviourSingleton<BackgroundMusic>
{
    private AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.Play();
    }
}

