using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Menu Settings")]
    public GameObject[] menus;    // An array of menus to toggle
    public GameObject[] otherObjects;  // Objects to hide/show
    public Button[] buttons;      // Buttons to trigger menu actions

    [Header("Main Menu Sorting Settings")]
    public Canvas[] mainMenuCanvases;  // Array of main menu canvases
    public int[] sortingOrders;        // Corresponding sorting orders for each canvas
    public Button[] changeOrderButtons;   // Buttons that trigger sorting order changes

    void Start()
    {
        // Setup buttons for toggling menus
        if (buttons != null && buttons.Length > 0)
        {
            foreach (Button btn in buttons)
            {
                if (btn != null)
                {
                    btn.onClick.AddListener(() => ToggleMenu(btn));
                }
                else
                {
                    Debug.LogError("A button is not assigned in the Inspector.");
                }
            }
        }
        else
        {
            Debug.LogError("Buttons array is not assigned or empty.");
        }

        // Ensure the initial state of the menus and other objects
        foreach (var menu in menus)
        {
            if (menu != null)
            {
                menu.SetActive(false); // Hide menus initially
            }
            else
            {
                Debug.LogError("One of the menus is not assigned in the Inspector.");
            }
        }

        foreach (var obj in otherObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true); // Ensure other objects are visible initially
            }
        }

        // Setup sorting order change buttons
        if (changeOrderButtons != null && changeOrderButtons.Length > 0)
        {
            for (int i = 0; i < changeOrderButtons.Length; i++)
            {
                if (changeOrderButtons[i] != null)
                {
                    int index = i;  // Capture the loop index to use in the listener
                    changeOrderButtons[i].onClick.AddListener(() => ChangeMainMenuSortingOrder(index));
                }
                else
                {
                    Debug.LogError("One of the change order buttons is not assigned.");
                }
            }
        }
        else
        {
            Debug.LogError("Change Order Buttons array is not assigned or empty.");
        }
    }

    // Toggle menu visibility when button is pressed
    void ToggleMenu(Button pressedButton)
    {
        // Find the index of the button that was pressed
        int index = System.Array.IndexOf(buttons, pressedButton);
        if (index == -1)
        {
            Debug.LogError("Button not found in the buttons array.");
            return;
        }

        if (index < menus.Length)
        {
            GameObject menuToToggle = menus[index];
            if (menuToToggle != null)
            {
                bool isMenuActive = menuToToggle.activeSelf;
                menuToToggle.SetActive(!isMenuActive);

                // Hide or show other objects based on the menu's visibility
                foreach (var obj in otherObjects)
                {
                    if (obj != null)
                    {
                        obj.SetActive(isMenuActive); // Show if menu is active, hide if not
                    }
                }

                // Force the canvas to update immediately (ensures visibility updates)
                Canvas.ForceUpdateCanvases();
            }
            else
            {
                Debug.LogError("Menu not assigned for button index " + index);
            }
        }
        else
        {
            Debug.LogError("Button index exceeds number of available menus.");
        }
    }

    // Change the sorting order of the selected main menu canvas
    void ChangeMainMenuSortingOrder(int index)
    {
        if (mainMenuCanvases != null && mainMenuCanvases.Length > 0 && sortingOrders != null && sortingOrders.Length > 0)
        {
            if (index >= 0 && index < mainMenuCanvases.Length && index < sortingOrders.Length)
            {
                Canvas canvasToChange = mainMenuCanvases[index];
                int newSortingOrder = sortingOrders[index];

                if (canvasToChange != null)
                {
                    canvasToChange.sortingOrder = newSortingOrder;
                    Debug.Log("Main Menu Canvas " + index + " sorting order changed to: " + newSortingOrder);
                }
                else
                {
                    Debug.LogError("Canvas not assigned for index " + index);
                }
            }
            else
            {
                Debug.LogError("Index out of bounds for mainMenuCanvases or sortingOrders arrays.");
            }
        }
        else
        {
            Debug.LogError("Main Menu Canvases or Sorting Orders array is not assigned or empty.");
        }
    }
}
