using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveButtonController : MonoBehaviour
{
    public Button saveButton;

    private void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 3) // If in AI Testing Grounds (scene 3)
        {
            saveButton.interactable = false; // Disable save button
        }
        else
        {
            saveButton.interactable = true; // Otherwise allow saving
        }
    }

    public void OnSavePressed()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex != 3) // Double safety check
        {
            SaveManager.Instance.SaveGame(sceneIndex);
        }
    }
}
