using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public int savedScene = 1; // Default to Scene 1
    public bool hasSaved = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NewGame()
    {
        hasSaved = false;
        savedScene = 1; // Start with Scene 1
        SceneManager.LoadScene(savedScene);
    }

    public void SaveGame(int sceneNumber)
    {
        savedScene = sceneNumber;
        hasSaved = true;
    }

    public void ContinueGame()
    {
        if (hasSaved)
        {
            SceneManager.LoadScene(savedScene);
        }
    }
}
