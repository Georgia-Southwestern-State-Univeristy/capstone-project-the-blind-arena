using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VendorInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private float promptHeightOffset = 2f; // Height above vendor

    [Header("Prompt Position Fine-tuning")]
    [SerializeField] private float horizontalOffset = 0f; // Positive = right, Negative = left
    [SerializeField] private float verticalOffset = 0f; // Fine-tune vertical position

    [Header("References")]
    [SerializeField] private RectTransform buttonPrompt; // UI element showing "Press E to interact"
    [SerializeField] private GameObject vendorMenu; // The vendor's menu to show/hide
    [SerializeField] private Canvas canvas; // Reference to the UI canvas

    private Transform playerTransform;
    private bool isInRange;
    private VisibilityObjects menuVisibility;
    private Camera mainCamera;
    private bool isMenuVisible = false; // Track menu state explicitly

    private void Start()
    {
        // Find the player in the scene
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;

        // If canvas reference is missing, try to find it
        if (canvas == null && buttonPrompt != null)
        {
            canvas = buttonPrompt.GetComponentInParent<Canvas>();
        }

        // Get the VisibilityObjects component from the menu
        if (vendorMenu != null)
        {
            menuVisibility = vendorMenu.GetComponent<VisibilityObjects>();
            if (menuVisibility == null)
            {
                menuVisibility = vendorMenu.AddComponent<VisibilityObjects>();
            }
            // Ensure menu starts hidden
            menuVisibility.SetVisibility(false);
            isMenuVisible = false;
            vendorMenu.SetActive(false);
        }

        // Hide the button prompt initially
        if (buttonPrompt != null)
            buttonPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerTransform == null || mainCamera == null) return;

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool wasInRange = isInRange;
        isInRange = distanceToPlayer <= interactionDistance;

        // Show/hide button prompt based on distance
        if (isInRange != wasInRange)
        {
            if (buttonPrompt != null)
                buttonPrompt.gameObject.SetActive(isInRange);

            // If player walks away, hide the menu
            if (!isInRange && isMenuVisible)
            {
                HideMenu();
            }
        }

        // Update prompt position if visible
        if (isInRange && buttonPrompt != null && canvas != null)
        {
            UpdatePromptPosition();
        }

        // Handle interaction
        if (isInRange && Input.GetKeyDown(interactionKey))
        {
            ToggleMenu();
        }
    }

    private void UpdatePromptPosition()
    {
        // Calculate position above vendor
        Vector3 worldPosition = transform.position + Vector3.up * promptHeightOffset;

        // Convert world position to screen position
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        // Check if behind camera
        if (screenPosition.z < 0)
        {
            buttonPrompt.gameObject.SetActive(false);
            return;
        }

        // Convert screen position to overlay position
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchoredPosition = new Vector2(
            (screenPosition.x / screenSize.x) * screenSize.x - (screenSize.x * 0.5f),
            (screenPosition.y / screenSize.y) * screenSize.y - (screenSize.y * 0.5f)
        );

        // Apply the offset
        anchoredPosition += new Vector2(horizontalOffset, verticalOffset);

        // Update UI position
        buttonPrompt.anchoredPosition = anchoredPosition;
        buttonPrompt.gameObject.SetActive(true);
    }

    private void ToggleMenu()
    {
        if (vendorMenu == null) return;

        isMenuVisible = !isMenuVisible;
        vendorMenu.SetActive(isMenuVisible);
        if (menuVisibility != null)
        {
            menuVisibility.SetVisibility(isMenuVisible);
        }
    }

    private void HideMenu()
    {
        if (vendorMenu == null) return;

        isMenuVisible = false;
        vendorMenu.SetActive(false);
        if (menuVisibility != null)
        {
            menuVisibility.SetVisibility(false);
        }
    }

    // Optional: Visualize the interaction range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        // Visualize prompt position
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * promptHeightOffset);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * promptHeightOffset, 0.1f);
    }
}
