using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button continueButton;

    private void Start()
    {
        UpdateContinueButton();
    }

    public void OnNewGamePressed()
    {
        SaveManager.Instance.NewGame();
    }

    public void OnContinuePressed()
    {
        SaveManager.Instance.ContinueGame();
    }

    private void UpdateContinueButton()
    {
        continueButton.interactable = SaveManager.Instance.hasSaved;
    }
}

