using UnityEngine;

public class QuitButton : MonoBehaviour
{
    // This method should be assigned to the Quit Button's OnClick event
    public void QuitGame()
    {
        Debug.Log("Quit button pressed. Exiting the game...");

        // Quit the application
        Application.Quit();

        // Note: Application.Quit() won't work in the editor. This log is for testing in the editor.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}