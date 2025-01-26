using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // This method is called when the quit button is pressed
    public void Quit()
    {
        // Log a message for debugging in the editor
        Debug.Log("Quit button pressed. Exiting game...");

        // Quit the application
        Application.Quit();
    }
}