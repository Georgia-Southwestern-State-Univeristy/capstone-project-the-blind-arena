using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public int nextSceneIndex = 1; // Set the next scene index in the inspector
    public Button skipButton;
    private bool isScriptFinished = false;

    void Start()
    {
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipScene);
        }

        StartCoroutine(ExecuteScript());
    }

    IEnumerator ExecuteScript()
    {
        // Simulate waiting for an external event instead of using a timer
        while (!isScriptFinished)
        {
            yield return null; // Wait until the script finishes naturally
        }

        TransitionToNextScene();
    }

    public void FinishScript()
    {
        isScriptFinished = true;
        TransitionToNextScene();
    }

    public void SkipScene()
    {
        isScriptFinished = true;
        TransitionToNextScene();
    }

    void TransitionToNextScene()
    {
        if (nextSceneIndex >= 0 && nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Scene index is out of range! Check Build Settings.");
        }
    }
}
