using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VendorInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactionKey = KeyCode.Q;
    [SerializeField] private float promptHeightOffset = 2f;

    [Header("UI References")]
    [SerializeField] private RectTransform buttonPrompt;
    [SerializeField] private GameObject vendorMenu;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button leaveButton;
    [SerializeField] private GameObject greetingDialogue;
    [SerializeField] private GameObject leaveDialogue;

    [Header("Prompt Offsets")]
    [SerializeField] private float horizontalOffset = 0f;
    [SerializeField] private float verticalOffset = 0f;

    private Transform playerTransform;
    private Camera mainCamera;
    private bool isInRange;
    private bool isMenuVisible;
    private bool greetingShown;
    private bool leavingShown;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        mainCamera = Camera.main;

        if (canvas == null && buttonPrompt != null)
            canvas = buttonPrompt.GetComponentInParent<Canvas>();

        vendorMenu?.SetActive(false);
        buttonPrompt?.gameObject.SetActive(false);

        if (leaveButton != null)
            leaveButton.onClick.AddListener(OnLeaveButtonClicked);
    }

    private void Update()
    {
        if (playerTransform == null || mainCamera == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasInRange = isInRange;
        isInRange = distance <= interactionDistance;

        if (isInRange != wasInRange)
        {
            buttonPrompt?.gameObject.SetActive(isInRange);
            if (!isInRange && isMenuVisible)
                HideMenu();
        }

        if (isInRange && buttonPrompt != null)
            UpdatePromptPosition();

        if (isInRange && Input.GetKeyDown(interactionKey))
        {
            if (!isMenuVisible && !greetingShown)
            {
                greetingShown = true;
                greetingDialogue?.SetActive(true);
                StartCoroutine(OpenMenuAfterDelay(3.5f));
            }
            else if (isMenuVisible)
            {
                HideMenu();
            }
        }
    }

    private IEnumerator OpenMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleMenu(true);
        greetingDialogue?.SetActive(false);
    }

    private void UpdatePromptPosition()
    {
        Vector3 worldPos = transform.position + Vector3.up * promptHeightOffset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0)
        {
            buttonPrompt?.gameObject.SetActive(false);
            return;
        }

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchoredPos = new Vector2(
            (screenPos.x / screenSize.x) * screenSize.x - (screenSize.x * 0.5f),
            (screenPos.y / screenSize.y) * screenSize.y - (screenSize.y * 0.5f)
        );

        anchoredPos += new Vector2(horizontalOffset, verticalOffset);
        buttonPrompt.anchoredPosition = anchoredPos;
        buttonPrompt.gameObject.SetActive(true);
    }

    private void ToggleMenu(bool show)
    {
        isMenuVisible = show;
        vendorMenu?.SetActive(show);
        Time.timeScale = show ? 0 : 1;
    }

    public void HideMenu()
    {
        ToggleMenu(false);
        greetingDialogue?.SetActive(false);
        leaveDialogue?.SetActive(false);
        greetingShown = false;
        leavingShown = true;
        leaveDialogue.SetActive(true);
    }

    public void OnLeaveButtonClicked()
    {
        if (isInRange && isMenuVisible && !leavingShown)
        {
            leaveDialogue?.SetActive(true);
            leavingShown = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * promptHeightOffset);
        Gizmos.DrawWireSphere(transform.position + Vector3.up * promptHeightOffset, 0.1f);
    }
}
