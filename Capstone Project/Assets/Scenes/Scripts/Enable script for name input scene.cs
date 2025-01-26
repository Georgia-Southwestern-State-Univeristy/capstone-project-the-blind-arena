using UnityEngine;
using UnityEngine.UI;

public class EnableTimedFadingLooper : MonoBehaviour
{
    // Reference to the TimedFadingLooper script to enable
    public TimedFadingLooper fadingLooperScript;

    // Reference to the Button that will enable the script
    public Button enableButton;

    void Start()
    {
        // Ensure references are assigned
        if (fadingLooperScript == null)
        {
            Debug.LogError("FadingLooperScript is not assigned!");
        }
        else
        {
            // Disable the TimedFadingLooper script initially
            fadingLooperScript.enabled = false;
        }

        if (enableButton == null)
        {
            Debug.LogError("EnableButton is not assigned!");
        }
        else
        {
            // Attach the EnableScript method to the button's onClick event
            enableButton.onClick.AddListener(EnableScript);
        }
    }

    void EnableScript()
    {
        if (fadingLooperScript != null)
        {
            // Enable the TimedFadingLooper script
            fadingLooperScript.enabled = true;
            Debug.Log("TimedFadingLooper has been enabled.");
        }
    }
}