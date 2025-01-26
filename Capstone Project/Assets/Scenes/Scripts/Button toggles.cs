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

        // Hide the options menu initially
        optionsMenu.SetActive(false);

        // Hide all other objects initially
        foreach (var obj in otherObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true); // Make sure other objects are visible initially
            }
        }
    }

    void ToggleOptionsMenu()
    {
        // Toggle the visibility of the options menu
        bool isMenuActive = optionsMenu.activeSelf;
        optionsMenu.SetActive(!isMenuActive);

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