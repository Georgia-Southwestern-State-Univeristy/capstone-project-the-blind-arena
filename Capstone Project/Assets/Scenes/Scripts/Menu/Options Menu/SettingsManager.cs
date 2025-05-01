using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] musicSources;
    [SerializeField] private AudioSource[] soundSources;

    private float savedMusicVolume;
    private float savedSoundVolume;

    void Awake()
    {
        // If there is already a SettingsManager, destroy this one
        if (FindFirstObjectByType<SettingsManager>() != null && FindFirstObjectByType<SettingsManager>() != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Initialize sliders and listeners
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
            musicSlider.value = savedMusicVolume;
        }

        if (soundSlider != null)
        {
            soundSlider.onValueChanged.AddListener(UpdateSoundVolume);
            soundSlider.value = savedSoundVolume;
        }
    }

    void LoadSettings()
    {
        // Load saved values from PlayerPrefs
        savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        ApplySettings();
    }

    void ApplySettings()
    {
        // Apply loaded settings to the audio sources
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

        // Optionally, save the current settings to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", savedMusicVolume);
        PlayerPrefs.SetFloat("SoundVolume", savedSoundVolume);
        PlayerPrefs.Save();
    }

    void UpdateMusicVolume(float value)
    {
        savedMusicVolume = value;
        ApplySettings();
    }

    void UpdateSoundVolume(float value)
    {
        savedSoundVolume = value;
        ApplySettings();
    }
}

