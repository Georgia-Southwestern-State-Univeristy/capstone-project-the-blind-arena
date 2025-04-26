using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundSettings : MonoBehaviour
{
    public Slider musicSlider; // Assign the music volume slider in the inspector
    public Slider soundSlider; // Assign the sound effects volume slider in the inspector
    public Button saveButton; // Assign the save button in the inspector

    [Header("Audio Sources")]
    [SerializeField] private AudioSource[] musicSources; // Music sources
    [SerializeField] private AudioSource[] soundSources; // Sound effect sources

    [Header("GameObjects Without AudioClips")]
    [SerializeField] private GameObject[] nonClipObjects; // Objects that don't use AudioClips

    private float savedMusicVolume;
    private float savedSoundVolume;

    private bool settingsSaved; // Tracks whether the Save button was pressed

    void Start()
    {
        LoadSettings();

        saveButton.onClick.AddListener(ApplySettings);

        musicSlider.onValueChanged.AddListener(UpdateMusicVolume);
        soundSlider.onValueChanged.AddListener(UpdateSoundVolume);

        settingsSaved = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadSettings(); // Reload saved settings when a scene is loaded
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void UpdateMusicVolume(float value)
    {
        foreach (AudioSource source in musicSources)
        {
            if (source != null)
            {
                source.volume = value;
            }
        }
    }

    void UpdateSoundVolume(float value)
    {
        foreach (AudioSource source in soundSources)
        {
            if (source != null)
            {
                source.volume = value;
            }
        }
    }

    void ApplySettings()
    {
        UpdateMusicVolume(musicSlider.value);
        UpdateSoundVolume(soundSlider.value);

        // Optionally you could adjust something for nonClipObjects here if needed.

        // Save the settings
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.Save();

        savedMusicVolume = musicSlider.value;
        savedSoundVolume = soundSlider.value;

        settingsSaved = true;

        Debug.Log("Settings saved: Music = " + musicSlider.value + ", Sound = " + soundSlider.value);
    }

    void RevertSettings()
    {
        if (!settingsSaved)
        {
            musicSlider.value = savedMusicVolume;
            soundSlider.value = savedSoundVolume;

            UpdateMusicVolume(savedMusicVolume);
            UpdateSoundVolume(savedSoundVolume);

            Debug.Log("Settings reverted: Music = " + savedMusicVolume + ", Sound = " + savedSoundVolume);
        }
        else
        {
            Debug.Log("Settings were saved; no revert needed.");
        }
    }

    void LoadSettings()
    {
        savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        savedSoundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        musicSlider.value = savedMusicVolume;
        soundSlider.value = savedSoundVolume;

        UpdateMusicVolume(savedMusicVolume);
        UpdateSoundVolume(savedSoundVolume);
    }
}
