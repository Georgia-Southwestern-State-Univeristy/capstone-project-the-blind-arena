using UnityEngine;
using UnityEngine.UI;

public class FirstTimeSetup : MonoBehaviour
{
    [Header("Default Slider Values")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;

    private bool isFirstTime;

    void Start()
    {
        // Check if this is the first time the game is launched
        isFirstTime = !PlayerPrefs.HasKey("MusicVolume") || !PlayerPrefs.HasKey("SoundVolume");

        if (isFirstTime)
        {
            // Set default values for sliders on the first launch
            musicSlider.value = 0.5f;  // Default music volume (can be customized)
            soundSlider.value = 0.5f;  // Default sound volume (can be customized)

            // Save these default values in PlayerPrefs
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
            PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
            PlayerPrefs.Save();

            Debug.Log("First time running! Default settings applied.");
        }
        else
        {
            // Load saved settings if it's not the first time
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        }

        // Apply audio settings based on loaded or default values
        ApplyAudioSettings();
    }

    void ApplyAudioSettings()
    {
        // Apply the values to music and sound volume here, assuming you have AudioSources
        AudioSource[] musicSources = FindObjectsOfType<AudioSource>();
        foreach (var source in musicSources)
        {
            if (source != null)
                source.volume = musicSlider.value;
        }
    }
}

