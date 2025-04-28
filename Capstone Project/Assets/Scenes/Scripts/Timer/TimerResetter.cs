using UnityEngine;
using UnityEngine.UI;

public class TimerResetter : MonoBehaviour
{
    [SerializeField] private Button resetButton;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] public TutorialSetup tutorialSetup;

    private void Start()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetGame);

        if (resetButton == null || gameTimer == null || tutorialSetup == null)
            Debug.LogWarning("TimerResetter is missing one or more references!");
    }

    private void ResetGame()
    {
        gameTimer.ResetTimer();
        ResetTutorialProgress();
        GameData.deathcounter = 0;
    }

    private void ResetTutorialProgress()
    {
        tutorialSetup.tutorialcounter = 0;

        // Reset all relevant tutorial flags if needed
        typeof(TutorialSetup)
            .GetField("didLeftClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(tutorialSetup, false);

        typeof(TutorialSetup)
            .GetField("didRightClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(tutorialSetup, false);

        typeof(TutorialSetup)
            .GetField("didPressE", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(tutorialSetup, false);

        typeof(TutorialSetup)
            .GetField("didPressSpace", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(tutorialSetup, false);

        typeof(TutorialSetup)
            .GetField("startedAttributeTutorial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(tutorialSetup, false);

        Debug.Log("Tutorial progress and timer reset.");

        typeof(TutorialSetup)
    .GetField("tutorialBossActivated", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
    ?.SetValue(tutorialSetup, false);

    }
}

