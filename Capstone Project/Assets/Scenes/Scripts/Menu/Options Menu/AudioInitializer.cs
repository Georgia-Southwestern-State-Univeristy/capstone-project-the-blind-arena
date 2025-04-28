using UnityEngine;

public class AudioInitializer : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] musicSources; // Assign music AudioSources
    [SerializeField] private AudioSource[] soundSources; // Assign sound effect AudioSources

    void Start()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        foreach (AudioSource source in musicSources)
        {
            if (source != null)
                source.volume = savedMusicVolume;
        }

        foreach (AudioSource source in soundSources)
        {
            if (source != null)
                source.volume = savedSoundVolume;
        }
    }
}
