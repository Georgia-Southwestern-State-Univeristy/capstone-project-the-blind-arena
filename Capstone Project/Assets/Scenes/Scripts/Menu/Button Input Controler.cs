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
        HandleAbilities();
        HandleItems();
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

    private void HandleAbilities()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Ability Q Pressed");
            UseAbility("Ability Q");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Ability E Pressed");
            UseAbility("Ability E");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Ability X Pressed");
            UseAbility("Ability X");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Ability C Pressed");
            UseAbility("Ability C");
        }
    }

    private void UseAbility(string abilityName)
    {
        Debug.Log($"Using {abilityName}");
        // Add your ability logic here
    }

    private void HandleItems()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Item 1 Pressed");
            UseItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Item 2 Pressed");
            UseItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Item 3 Pressed");
            UseItem(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Item 4 Pressed");
            UseItem(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Item 5 Pressed");
            UseItem(5);
        }
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
        Time.timeScale = 1f; // Temporarily resume time for UI
        inventoryUI.SetActive(true); // Show the inventory UI
        Canvas.ForceUpdateCanvases(); // Force canvas update immediately
        isInventoryOpen = true;
    }

    private void CloseInventory()
    {
        Debug.Log("Inventory Closed");
        Time.timeScale = 1f; // Temporarily resume time for UI
        inventoryUI.SetActive(false); // Hide the inventory UI
        Canvas.ForceUpdateCanvases(); // Force canvas update immediately
        isInventoryOpen = false;
    }
}