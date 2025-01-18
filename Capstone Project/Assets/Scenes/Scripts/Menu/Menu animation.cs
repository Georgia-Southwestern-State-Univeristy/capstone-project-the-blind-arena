using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuAnimation : MonoBehaviour
{
    // Assign UI elements in the Inspector
    public Button startGameButton;
    public Button settingsButton;
    public Button quitButton;
    public RectTransform background;

    private Vector3 initialBackgroundPosition;

    private void Start()
    {
        // Store the initial position of the background
        if (background != null)
        {
            initialBackgroundPosition = background.position;
        }

        // Add listeners for button interactions
        startGameButton.onClick.AddListener(() => LoadScene("GameScene"));
        settingsButton.onClick.AddListener(() => OpenSettings());
        quitButton.onClick.AddListener(() => QuitGame());

        // Add hover effects to buttons
        AddHoverEffect(startGameButton);
        AddHoverEffect(settingsButton);
        AddHoverEffect(quitButton);
    }

    private void Update()
    {
        // Animate background to move based on mouse position
        if (background != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            float xOffset = (mousePosition.x / Screen.width - 0.5f) * 50f; // Reduced scale for mouse X position
            float yOffset = (mousePosition.y / Screen.height - 0.5f) * 50f; // Reduced scale for mouse Y position
            background.position = new Vector3(initialBackgroundPosition.x + xOffset, initialBackgroundPosition.y + yOffset, initialBackgroundPosition.z);
        }
    }

    private void AddHoverEffect(Button button)
    {
        ColorBlock colorBlock = button.colors;
        Color normalColor = colorBlock.normalColor;
        Color highlightedColor = new Color(1f, 0.8f, 0.5f); // Slight orange tint

        button.onClick.AddListener(() => Debug.Log(button.name + " clicked!"));

        // Change color on hover
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => button.GetComponent<Image>().color = highlightedColor);
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) => button.GetComponent<Image>().color = normalColor);
        trigger.triggers.Add(entryExit);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OpenSettings()
    {
        Debug.Log("Settings menu opened.");
        // Add logic to open settings menu here
    }

    private void QuitGame()
    {
        Debug.Log("Quit game pressed.");
        Application.Quit();
    }
}

