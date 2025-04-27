using UnityEngine;

public class SceneSoundManager : MonoBehaviour
{
    [Header("Audio Sources for this Scene")]
    [SerializeField] private AudioSource[] musicSources;  // Music sources for this scene
    [SerializeField] private AudioSource[] soundSources;  // Sound effect sources for this scene

    private float currentMusicVolume;
    private float currentSoundVolume;

    void Start()
    {
        // Get saved settings from PlayerPrefs
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        currentSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        // Apply audio settings for this scene
        ApplyAudioSettings();
    }

    void ApplyAudioSettings()
    {
        // Apply volume to music sources
        foreach (var source in musicSources)
        {
            if (source != null)
                source.volume = currentMusicVolume;
        }

        // Apply volume to sound effect sources
        foreach (var source in soundSources)
        {
            if (source != null)
                source.volume = currentSoundVolume;
        }
    }

    public void UpdateMusicVolume(float value)
    {
        currentMusicVolume = value;
        ApplyAudioSettings();

        // Save the updated setting
        PlayerPrefs.SetFloat("MusicVolume", currentMusicVolume);
        PlayerPrefs.Save();
    }

    public void UpdateSoundVolume(float value)
    {
        currentSoundVolume = value;
        ApplyAudioSettings();

        // Save the updated setting
        PlayerPrefs.SetFloat("SoundVolume", currentSoundVolume);
        PlayerPrefs.Save();
    }
}

