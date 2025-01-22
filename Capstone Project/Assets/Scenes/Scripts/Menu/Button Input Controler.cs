using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; // Drag your Pause Menu UI GameObject here
    [SerializeField] private GameObject inventoryUI; // Drag your Inventory UI GameObject here

    private bool isPaused = false;
    private bool isInventoryOpen = false;

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


    private void UseItem(int itemNumber)
    {
        Debug.Log($"Using item {itemNumber}");
        // Add your item usage logic here
    }

    private void HandleInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I Pressed for Inventory");
            if (isInventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }

    private void OpenInventory()
    {
        Debug.Log("Inventory Opened");
        inventoryUI.SetActive(true); // Show the inventory UI
        Canvas.ForceUpdateCanvases(); // Force canvas update immediately
        isInventoryOpen = true;
    }

    private void CloseInventory()
    {
        Debug.Log("Inventory Closed");
        inventoryUI.SetActive(false); // Hide the inventory UI
        Canvas.ForceUpdateCanvases(); // Force canvas update immediately
        isInventoryOpen = false;
    }
}