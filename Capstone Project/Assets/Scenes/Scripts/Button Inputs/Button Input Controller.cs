using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;       // Drag your Pause Menu UI GameObject here
    [SerializeField] private GameObject inventoryUI;       // Drag your Inventory UI GameObject here
    [SerializeField] private GameObject attributeUI;       // Drag your Attribute UI GameObject here

    private bool isPaused = false;
    private bool isInventoryOpen = false;
    private bool isAttributeOpen = false;

    void Update()
    {
        HandlePauseMenu();
        HandleInventory();
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
        Time.timeScale = 0f; // Pause the game
        isPaused = true;
    }

    private void ResumeGame()
    {
        Debug.Log("Game Resumed");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        isPaused = false;
    }

    private void HandleInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I Pressed for Inventory and Attributes");
            if (isInventoryOpen || isAttributeOpen)
            {
                CloseAllMenus();
            }
            else
            {
                OpenAllMenus();
            }
        }
    }

    private void OpenAllMenus()
    {
        Debug.Log("Opening Inventory and Attributes");

        inventoryUI.SetActive(true);    // Show the inventory UI
        attributeUI.SetActive(true);    // Show the attribute UI

        Canvas.ForceUpdateCanvases();   // Force canvas update immediately

        isInventoryOpen = true;
        isAttributeOpen = true;
    }

    private void CloseAllMenus()
    {
        Debug.Log("Closing Inventory and Attributes");

        inventoryUI.SetActive(false);   // Hide the inventory UI
        attributeUI.SetActive(false);   // Hide the attribute UI

        Canvas.ForceUpdateCanvases();   // Force canvas update immediately

        isInventoryOpen = false;
        isAttributeOpen = false;
    }
}