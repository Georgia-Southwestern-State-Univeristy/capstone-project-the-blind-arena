using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Menu Settings")]
    public GameObject[] menus;           // Menus to toggle
    public GameObject[] otherObjects;    // Objects to hide/show
    public Button[] buttons;             // Buttons for toggling menus

    [Header("Main Menu Sorting Settings (Optional)")]
    public Canvas[] mainMenuCanvases;    // Canvases to reorder
    public int[] sortingOrders;          // Sorting order values
    public Button[] changeOrderButtons;  // Buttons to change sorting order

    void Start()
    {
        SetupMenuButtons();
        InitializeMenusAndObjects();
        SetupSortingOrderButtons(); // Now optional
    }

    private void SetupMenuButtons()
    {
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
                    Debug.LogError("A button in 'buttons' array is not assigned.");
                }
            }
        }
        else
        {
            Debug.LogWarning("No menu toggle buttons assigned.");
        }
    }

    private void InitializeMenusAndObjects()
    {
        foreach (var menu in menus)
        {
            if (menu != null)
                menu.SetActive(false);
            else
                Debug.LogWarning("A menu GameObject is not assigned.");
        }

        foreach (var obj in otherObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    private void SetupSortingOrderButtons()
    {
        if (changeOrderButtons != null && changeOrderButtons.Length > 0 &&
            mainMenuCanvases != null && sortingOrders != null &&
            mainMenuCanvases.Length == sortingOrders.Length &&
            changeOrderButtons.Length <= mainMenuCanvases.Length)
        {
            for (int i = 0; i < changeOrderButtons.Length; i++)
            {
                if (changeOrderButtons[i] != null)
                {
                    int index = i;
                    changeOrderButtons[i].onClick.AddListener(() => ChangeMainMenuSortingOrder(index));
                }
                else
                {
                    Debug.LogWarning($"Change order button at index {i} is not assigned.");
                }
            }
        }
        else
        {
            Debug.Log("Skipping sorting order setup (not used in this scene).");
        }
    }

    private void ToggleMenu(Button pressedButton)
    {
        int index = System.Array.IndexOf(buttons, pressedButton);
        if (index == -1)
        {
            Debug.LogError("Pressed button not found in 'buttons' array.");
            return;
        }

        if (index < menus.Length)
        {
            GameObject menu = menus[index];
            if (menu != null)
            {
                bool isActive = menu.activeSelf;
                menu.SetActive(!isActive);

                foreach (var obj in otherObjects)
                {
                    if (obj != null)
                        obj.SetActive(isActive); // Hide when menu is shown
                }

                Canvas.ForceUpdateCanvases();
            }
            else
            {
                Debug.LogWarning($"Menu at index {index} is not assigned.");
            }
        }
        else
        {
            Debug.LogError($"Index {index} out of bounds in 'menus' array.");
        }
    }

    private void ChangeMainMenuSortingOrder(int index)
    {
        if (index >= 0 && index < mainMenuCanvases.Length && index < sortingOrders.Length)
        {
            Canvas canvas = mainMenuCanvases[index];
            int newOrder = sortingOrders[index];

            if (canvas != null)
            {
                canvas.sortingOrder = newOrder;
                Debug.Log($"Canvas {index} sorting order set to {newOrder}.");
            }
            else
            {
                Debug.LogWarning($"Canvas at index {index} is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("Index out of range in mainMenuCanvases or sortingOrders.");
        }
    }
}
