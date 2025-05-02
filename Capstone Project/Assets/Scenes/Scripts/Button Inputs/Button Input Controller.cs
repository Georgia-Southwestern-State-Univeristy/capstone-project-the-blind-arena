using UnityEngine;
using UnityEngine.UI;

public class GameInputManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject attributeUI;

    [SerializeField] private Button closeAttributeUIButton; // Button to close Attribute UI
    [SerializeField] private Button closePauseUIButton;     // Button to close Pause Menu

    private bool isPaused = false;
    private bool isAttributeOpen = false;

    void Start()
    {
        if (closeAttributeUIButton != null)
        {
            closeAttributeUIButton.onClick.AddListener(CloseAttributeMenu);
        }
        else
        {
            Debug.LogWarning("Close Attribute Button not assigned.");
        }

        if (closePauseUIButton != null)
        {
            closePauseUIButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.LogWarning("Close Pause Button not assigned.");
        }
    }

    void Update()
    {
        HandlePauseMenu();
        HandleAttributeMenu();
    }

    private void HandlePauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape Pressed");
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        Debug.Log("Game Paused");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void ResumeGame()
    {
        Debug.Log("Game Resumed");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void HandleAttributeMenu()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I Pressed for Attributes");
            if (isAttributeOpen)
            {
                CloseAttributeMenu();
            }
            else
            {
                OpenAttributeMenu();
            }
        }
    }

    private void OpenAttributeMenu()
    {
        Debug.Log("Opening Attributes");
        attributeUI.SetActive(true);
        Canvas.ForceUpdateCanvases();
        isAttributeOpen = true;
    }

    private void CloseAttributeMenu()
    {
        Debug.Log("Closing Attributes");
        attributeUI.SetActive(false);
        Canvas.ForceUpdateCanvases();
        isAttributeOpen = false;
    }
}
