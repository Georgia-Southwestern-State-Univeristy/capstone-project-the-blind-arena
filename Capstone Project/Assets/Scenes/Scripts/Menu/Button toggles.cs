using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuToggle : MonoBehaviour
{
    public GameObject optionsMenu;    // The options menu to show/hide
    public GameObject[] otherObjects;  // Other objects to hide/show
    public Button optionsButton;       // The button to trigger the options menu

    void Start()
    {
        // Ensure the button is assigned and set up the click listener
        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(ToggleOptionsMenu);
        }
        else
        {
            Debug.LogError("Options button is not assigned in the Inspector.");
        }

        // Ensure the initial state of the options menu and other objects
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(false); // Hide options menu initially
        }
        else
        {
            Debug.LogError("Options menu is not assigned in the Inspector.");
        }

        foreach (var obj in otherObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true); // Ensure other objects are visible initially
            }
        }
    }

    void ToggleOptionsMenu()
    {
        if (optionsMenu == null)
        {
            Debug.LogError("Options menu is not assigned in the Inspector.");
            return;
        }

        // Toggle the visibility of the options menu
        bool isMenuActive = optionsMenu.activeSelf;
        optionsMenu.SetActive(!isMenuActive);

        // Force the canvas to update immediately (ensures visibility updates)
        Canvas.ForceUpdateCanvases();

        // Hide or show other objects based on the menu's visibility
        foreach (var obj in otherObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isMenuActive); // Show if menu is active, hide if not
            }
        }
    }
}