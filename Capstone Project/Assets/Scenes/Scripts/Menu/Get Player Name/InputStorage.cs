using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputStorage : MonoBehaviour
{
    public TMP_InputField userInputField; // Assign in the Inspector
    public Button saveInputButton; // Assign in the Inspector
    private const string InputKey = "StoredInput";

    void Start()
    {
        if (saveInputButton != null)
        {
            saveInputButton.onClick.AddListener(SaveInput);
        }
    }

    // Save the input to PlayerPrefs
    public void SaveInput()
    {
        string userInput = userInputField.text;
        PlayerPrefs.SetString(InputKey, userInput);
        PlayerPrefs.Save(); // Ensure the data is saved
        Debug.Log("Input saved: " + userInput);
    }
}