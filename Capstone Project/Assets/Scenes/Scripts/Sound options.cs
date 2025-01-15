using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    public Slider musicSlider; // Assign the music volume slider in the inspector
    public Slider soundSlider; // Assign the sound effects volume slider in the inspector
    public Button saveButton; // Assign the save button in the inspector
    public Button backButton; // Assign the back button in the inspector

    private AudioSource musicSource; // Reference to the music AudioSource
    private AudioSource soundSource; // Reference to the sound effects AudioSource

    private float savedMusicVolume;
    private float savedSoundVolume;

    private bool settingsSaved; // Tracks whether the Save button was pressed

    void Start()
    {
        // Initialize AudioSources (you can assign these in the inspector or find them dynamically)
        musicSource = GameObject.Find("MusicSource").GetComponent<AudioSource>();
        soundSource = GameObject.Find("SoundSource").GetComponent<AudioSource>();

        // Load saved values (if any)
        LoadSettings();

        // Add listeners to buttons
        saveButton.onClick.AddListener(ApplySettings);
        backButton.onClick.AddListener(RevertSettings);

        // Initially, no settings have been saved
        settingsSaved = false;
    }

    void ApplySettings()
    {
        // Set the AudioSource volumes based on slider positions
        if (musicSource != null)
        {
            musicSource.volume = musicSlider.value;
        }

        if (soundSource != null)
        {
            soundSource.volume = soundSlider.value;
        }

        // Save the settings
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.Save();

        // Update the saved values
        savedMusicVolume = musicSlider.value;
        savedSoundVolume = soundSlider.value;

        settingsSaved = true; // Mark the settings as saved

        Debug.Log("Settings saved: Music = " + musicSlider.value + ", Sound = " + soundSlider.value);
    }

    void RevertSettings()
    {
        if (!settingsSaved)
        {
            // Revert the sliders to their saved positions
            musicSlider.value = savedMusicVolume;
            soundSlider.value = savedSoundVolume;

            // Optionally revert the AudioSource volumes immediately
            if (musicSource != null)
            {
                musicSource.volume = savedMusicVolume;
            }

            if (soundSource != null)
            {
                soundSource.volume = savedSoundVolume;
            }

            Debug.Log("Settings reverted: Music = " + savedMusicVolume + ", Sound = " + savedSoundVolume);
        }
        else
        {
            Debug.Log("Settings were saved; no revert needed.");
        }
    }

    void LoadSettings()
    {
        // Load saved values or use defaults
        savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f); // Default to 0.5
        savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f); // Default to 0.5

        musicSlider.value = savedMusicVolume;
        soundSlider.value = savedSoundVolume;

        if (musicSource != null)
        {
            musicSource.volume = savedMusicVolume;
        }

        if (soundSource != null)
        {
            soundSource.volume = savedSoundVolume;
        }
    }
}