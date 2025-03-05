using UnityEngine;
using System.Collections;

public class UIHardCodeAnimation : MonoBehaviour
{
    public RectTransform uiElement; // Reference to the UI element
    public float animationDuration = 0.5f; // Duration of the animation
    private bool isOpen = false; // Track the state of the menu

    void Update()
    {
        // Check for the "E" key press
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isOpen)
            {
                CloseUI();
            }
            else
            {
                OpenUI();
            }
        }
    }

    public void OpenUI()
    {
        StartCoroutine(AnimateUI(new Vector3(0, 0, 0), 1f)); // Target position and scale for open
        isOpen = true; // Update the state
    }

    public void CloseUI()
    {
        StartCoroutine(AnimateUI(new Vector3(-1000, 0, 0), 0f)); // Target position and scale for close
        isOpen = false; // Update the state
    }

    private IEnumerator AnimateUI(Vector3 targetPosition, float targetAlpha)
    {
        Vector3 startPosition = uiElement.anchoredPosition;
        float startAlpha = uiElement.GetComponent<CanvasGroup>().alpha;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // Interpolate position and alpha
            uiElement.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t);
            uiElement.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null; // Wait for the next frame
        }

        // Ensure the final values are set
        uiElement.anchoredPosition = targetPosition;
        uiElement.GetComponent<CanvasGroup>().alpha = targetAlpha;
    }
}