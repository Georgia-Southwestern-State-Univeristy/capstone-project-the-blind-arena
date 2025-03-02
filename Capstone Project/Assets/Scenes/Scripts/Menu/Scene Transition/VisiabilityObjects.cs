using UnityEngine;

public class VisibilityObjects : MonoBehaviour
{
    private bool isVisible = true;

    public void SetVisibility(bool visible)
    {
        isVisible = visible;
        gameObject.SetActive(visible);
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        gameObject.SetActive(isVisible);
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}
