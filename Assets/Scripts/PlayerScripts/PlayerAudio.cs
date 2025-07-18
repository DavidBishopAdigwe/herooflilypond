
using UnityEngine;

public class PlayerAudio: MonoBehaviour
{
    [SerializeField] private AudioSource moveSound;
    [SerializeField] private AudioSource dragSound;
    private AudioSource _audio;
    private AudioClip _currentClip;
    
    private PlayerDrag _playerDrag;
    private PlayerPickup _playerPickup;

    private void Awake()
    {
        _playerPickup = GetComponent<PlayerPickup>();
        _playerDrag = GetComponent<PlayerDrag>();
    }

    public void PlayClip(AudioClip clip)
    {
        if (_audio.isPlaying)
        {
            _audio.Stop();
        }
        _currentClip = clip;
        _audio.clip = _currentClip;
        _audio.Play();
    }

    private void PlayPickupSound()
    {
        
    }

    public void StopClip(AudioClip clip)
    {
        if (_currentClip != null)
        {
            _audio.Stop();
        }
    }
} 

