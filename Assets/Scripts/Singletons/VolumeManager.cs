using UnityEngine;
using UnityEngine.Audio;

namespace Singletons
{
    [CreateAssetMenu(fileName = "VolumeManager", menuName = "ScriptableObjects/VolumeManager")]
    public class VolumeManager: ScriptableObjectSingleton<VolumeManager>
    {
        [SerializeField] private AudioMixer _mixer;

        public void SetVolume(string name, float value)
        {
            _mixer.SetFloat(name, value);
            PlayerPrefs.SetFloat(name, value);
        }

        public float GetVolume(string name)
        {
            return PlayerPrefs.GetFloat(name, 0f);
        }
    }
}