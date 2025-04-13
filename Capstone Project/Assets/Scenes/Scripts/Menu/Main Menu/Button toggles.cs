using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsMenuToggle : MonoBehaviour
{
    public GameObject optionsMenu;    // The options menu to show/hide
    public GameObject[] otherObjects; // Other objects to hide/show
    public Button optionsButton;      // The button to trigger the options menu
    public GameObject resumeObject;   // The specific object to resume time when clicked

    void Start()
    {
        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(ToggleOptionsMenu);
        }
        else
        {
            Debug.LogError("Options button is not assigned in the Inspector.");
        }

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

    void Update()
    {
        // Check if the mouse is clicked and if the clicked object is the resumeObject
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == resumeObject)
                {
                    ResumeGame();
                }
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

        bool isMenuActive = optionsMenu.activeSelf;
        optionsMenu.SetActive(!isMenuActive);

        // Pause the game when the menu is active
        Time.timeScale = isMenuActive ? 1 : 0;

        Canvas.ForceUpdateCanvases();

        foreach (var obj in otherObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isMenuActive);
            }
        }
    }

    void ResumeGame()
    {
        optionsMenu.SetActive(false);
        Time.timeScale = 1;

        foreach (var obj in otherObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
