using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputRetriever : MonoBehaviour
{
    public TMP_Text displayText; // Assign in the Inspector
    private const string InputKey = "StoredInput";

    void Start()
    {
        if (PlayerPrefs.HasKey(InputKey))
        {
            displayText.text = PlayerPrefs.GetString(InputKey);
        }
        else
        {
            displayText.text = "No input stored.";
        }
    }
}
