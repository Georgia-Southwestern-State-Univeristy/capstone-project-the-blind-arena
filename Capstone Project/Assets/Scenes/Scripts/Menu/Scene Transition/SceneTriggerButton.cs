using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Button menuButton;

    void Start()
    {
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(LoadMenuScene);
        }
        else
        {
            Debug.LogError("MenuButton is not assigned!");
        }
    }

    private void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }
}
