using System.Collections;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Canvas inventoryMenuCanvas;  // Drag your Inventory Canvas here
    [SerializeField] private Canvas skillsMenuCanvas;     // Drag your Skills Canvas here
    [SerializeField] private Canvas attributesMenuCanvas; // Drag your Attributes Canvas here

    private void Start()
    {
        // Initially hide the menus
        inventoryMenuCanvas.gameObject.SetActive(false);
        skillsMenuCanvas.gameObject.SetActive(false);
        attributesMenuCanvas.gameObject.SetActive(false);

        // Start the delay to unhide and reset sorting order
        StartCoroutine(UnhideMenusAfterDelay());
    }

    private IEnumerator UnhideMenusAfterDelay()
    {
        yield return new WaitForSeconds(3); // Wait for 3 seconds

        // Unhide the menus and set their initial sorting order to 5
        inventoryMenuCanvas.gameObject.SetActive(true);
        skillsMenuCanvas.gameObject.SetActive(true);
        attributesMenuCanvas.gameObject.SetActive(true);

        inventoryMenuCanvas.sortingOrder = 5;
        skillsMenuCanvas.sortingOrder = 5;
        attributesMenuCanvas.sortingOrder = 5;
    }

    public void ShowInventoryMenu()
    {
        // Set the inventory menu to the front
        Debug.Log("Inventory menu selected.");
        SetMenuSortingOrder(inventoryMenuCanvas);
    }

    public void ShowSkillsMenu()
    {
        // Set the skills menu to the front
        Debug.Log("Skills menu selected.");
        SetMenuSortingOrder(skillsMenuCanvas);
    }

    public void ShowAttributesMenu()
    {
        // Set the attributes menu to the front
        Debug.Log("Attributes menu selected.");
        SetMenuSortingOrder(attributesMenuCanvas);
    }

    private void SetMenuSortingOrder(Canvas targetMenu)
    {
        // Reset all menus to sorting order 5
        inventoryMenuCanvas.sortingOrder = 5;
        skillsMenuCanvas.sortingOrder = 5;
        attributesMenuCanvas.sortingOrder = 5;

        // Set the target menu to sorting order 30
        targetMenu.sortingOrder = 30;
    }
}
