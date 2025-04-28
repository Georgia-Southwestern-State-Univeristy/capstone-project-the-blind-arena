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
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
