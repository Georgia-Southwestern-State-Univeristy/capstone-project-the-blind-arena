using UnityEngine;
using UnityEngine.UI;

public class LaptopScreenScaler : MonoBehaviour
{
    [Tooltip("The reference width of the design resolution.")]
    public float referenceWidth = 1920f;

    [Tooltip("The reference height of the design resolution.")]
    public float referenceHeight = 1080f;

    private CanvasScaler canvasScaler;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        if (canvasScaler == null)
        {
            Debug.LogWarning("LaptopScreenScaler requires a CanvasScaler component.");
            return;
        }

        // Check if the device is likely a laptop
        if (IsLaptopScreen())
        {
            AdjustScale();
        }
        else
        {
            Debug.Log("Screen does not match typical laptop dimensions. No scaling applied.");
        }
    }

    void AdjustScale()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate the scaling factor
        float widthRatio = screenWidth / referenceWidth;
        float heightRatio = screenHeight / referenceHeight;

        // Configure the Canvas Scaler
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(referenceWidth, referenceHeight);
        canvasScaler.matchWidthOrHeight = (widthRatio > heightRatio) ? 1 : 0;

        Debug.Log($"Scaling applied for laptop screen: {Screen.width}x{Screen.height}");
    }

    bool IsLaptopScreen()
    {
        // Check common laptop resolutions and aspect ratios
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float aspectRatio = screenWidth / screenHeight;

        // Common laptop resolutions and aspect ratios
        return (screenWidth == 1366 && screenHeight == 768) || // 1366x768
               (screenWidth == 1920 && screenHeight == 1080) || // 1920x1080
               (aspectRatio > 1.3f && aspectRatio < 1.8f && screenHeight >= 720); // General laptop aspect ratio and height
    }
}