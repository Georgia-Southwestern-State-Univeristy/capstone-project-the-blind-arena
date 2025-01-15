using UnityEngine;
using UnityEngine.UI;

public class MainMenuSortOrder : MonoBehaviour
{
    // Reference to the Canvas of the Main Menu
    public Canvas mainMenuCanvas;

    // New sorting order to apply
    public int newSortingOrder;

    // Button that triggers the sorting order change
    public Button changeOrderButton;

    void Start()
    {
        // Ensure references are assigned
        if (mainMenuCanvas == null)
        {
            Debug.LogError("Main Menu Canvas is not assigned!");
        }

        if (changeOrderButton == null)
        {
            Debug.LogError("Change Order Button is not assigned!");
        }
        else
        {
            // Add a listener to the button
            changeOrderButton.onClick.AddListener(ChangeMainMenuSortingOrder);
        }
    }

    void ChangeMainMenuSortingOrder()
    {
        if (mainMenuCanvas != null)
        {
            // Change the sort order of the main menu's Canvas
            mainMenuCanvas.sortingOrder = newSortingOrder;
            Debug.Log("Main Menu sorting order changed to: " + newSortingOrder);
        }
    }
}
