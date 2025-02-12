using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputStorage : MonoBehaviour
{
    public TMP_InputField userInputField; // Assign in the Inspector
    public Button loadSceneButton; // Assign in the Inspector
    private const string InputKey = "StoredInput";

    void Start()
    {
        if (loadSceneButton != null)
        {
            loadSceneButton.onClick.AddListener(() => LoadNextScene("NextScene")); // Change "NextScene" to your scene name
        }
    }

    // Save the input to PlayerPrefs
    public void SaveInput()
    {
        string userInput = userInputField.text;
        PlayerPrefs.SetString(InputKey, userInput);
        PlayerPrefs.Save(); // Ensure the data is saved
    }

    // Load a new scene
    public void LoadNextScene(string sceneName)
    {
        SaveInput();
        SceneManager.LoadScene(sceneName);
    }
}