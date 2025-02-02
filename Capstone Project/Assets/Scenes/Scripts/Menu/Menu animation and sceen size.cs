using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LaptopMenuManager : MonoBehaviour
{
    [Tooltip("The reference width of the design resolution.")]
    public float referenceWidth = 1920f;

    [Tooltip("The reference height of the design resolution.")]
    public float referenceHeight = 1080f;

    public Button startGameButton;
    public Button settingsButton;
    public Button quitButton;
    public RectTransform background;

    private CanvasScaler canvasScaler;
    private Vector3 initialBackgroundPosition;

    private void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        if (canvasScaler != null && IsLaptopScreen())
        {
            AdjustScale();
        }

        if (background != null)
        {
            initialBackgroundPosition = background.position;
        }

        startGameButton.onClick.AddListener(() => LoadScene("GameScene"));
        settingsButton.onClick.AddListener(() => OpenSettings());
        quitButton.onClick.AddListener(() => QuitGame());

        AddHoverEffect(startGameButton);
        AddHoverEffect(settingsButton);
        AddHoverEffect(quitButton);
    }

    private void Update()
    {
        if (background != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            float xOffset = (mousePosition.x / Screen.width - 0.5f) * 50f;
            float yOffset = (mousePosition.y / Screen.height - 0.5f) * 50f;
            background.position = new Vector3(initialBackgroundPosition.x + xOffset, initialBackgroundPosition.y + yOffset, initialBackgroundPosition.z);
        }
    }

    private void AdjustScale()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float widthRatio = screenWidth / referenceWidth;
        float heightRatio = screenHeight / referenceHeight;

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(referenceWidth, referenceHeight);
        canvasScaler.matchWidthOrHeight = (widthRatio > heightRatio) ? 1 : 0;

        Debug.Log($"Scaling applied for laptop screen: {Screen.width}x{Screen.height}");
    }

    private bool IsLaptopScreen()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float aspectRatio = screenWidth / screenHeight;

        return (screenWidth == 1366 && screenHeight == 768) ||
               (screenWidth == 1920 && screenHeight == 1080) ||
               (aspectRatio > 1.3f && aspectRatio < 1.8f && screenHeight >= 720);
    }

    private void AddHoverEffect(Button button)
    {
        ColorBlock colorBlock = button.colors;
        Color normalColor = colorBlock.normalColor;
        Color highlightedColor = new Color(1f, 0.8f, 0.5f);

        button.onClick.AddListener(() => Debug.Log(button.name + " clicked!"));

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
    }

    private void QuitGame()
    {
        Debug.Log("Quit game pressed.");
        Application.Quit();
    }
}
