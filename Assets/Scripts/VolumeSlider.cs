
using Managers.BaseManagers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VolumeSlider: MonoBehaviourSingleton<VolumeSlider>
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider, musicSlider, sfxSlider;
    
    private const float MinDb = -80f;
    private const float MaxDb = 0f;

    private void Start()
    {
        masterSlider.minValue = MinDb; masterSlider.maxValue = MaxDb;
        musicSlider.minValue  = MinDb; musicSlider.maxValue  = MaxDb;
        sfxSlider.minValue    = MinDb; sfxSlider.maxValue    = MaxDb;

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0f);
        musicSlider.value  = PlayerPrefs.GetFloat("MusicVolume",  0f);
        sfxSlider.value    = PlayerPrefs.GetFloat("SFXVolume",    0f);
    }

    public void SetMasterVolume(float db)
    {
        mixer.SetFloat("MasterVolume", db);
        PlayerPrefs.SetFloat("MasterVolume", db);
    }

    public void SetMusicVolume(float db)
    {
        mixer.SetFloat("MusicVolume", db);
        PlayerPrefs.SetFloat("MusicVolume", db);
    }

    public void SetSFXVolume(float db)
    {
        mixer.SetFloat("SFXVolume", db);
        PlayerPrefs.SetFloat("SFXVolume", db);
    }
}


