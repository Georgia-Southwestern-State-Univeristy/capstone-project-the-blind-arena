using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class DreamloManager : MonoBehaviour
{
    private string privateKey = "o_q7Avnx70aloLL6zdesYwtAGwCt";  // Keep this secret!
    private string publicKey = "67a3858a8f40bb16d8c13b61";  // Safe to share

    public TMP_InputField playerNameInput;  // TextMeshPro InputField
    public TMP_InputField scoreInput;       // TextMeshPro InputField
    public TMP_Text leaderboardText;        // TextMeshPro Text
    public Button submitButton;             // Button for submitting score

    void Start()
    {
        // Ensure the button is assigned and add a listener
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(AddScore);
        }
        else
        {
            Debug.LogError("Submit Button is not assigned in the Inspector!");
        }
    }

    // Function to Add Player Score to Dreamlo
    public void AddScore()
    {
        string playerName = playerNameInput.text;
        int score;

        if (int.TryParse(scoreInput.text, out score))
        {
            StartCoroutine(SendScore(playerName, score));
        }
        else
        {
            Debug.LogError("Invalid score input! Please enter a valid number.");
        }
    }

    private IEnumerator SendScore(string playerName, int score)
    {
        string url = $"http://dreamlo.com/lb/{privateKey}/add/{UnityWebRequest.EscapeURL(playerName)}/{score}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score added successfully!");
            }
            else
            {
                Debug.LogError("Error adding score: " + www.error);
            }
        }
    }

    // Function to Get Leaderboard from Dreamlo
    public void GetLeaderboard()
    {
        StartCoroutine(LoadLeaderboard());
    }

    private IEnumerator LoadLeaderboard()
    {
        string url = $"http://dreamlo.com/lb/{publicKey}/pipe";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                DisplayLeaderboard(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error fetching leaderboard: " + www.error);
            }
        }
    }

    private void DisplayLeaderboard(string data)
    {
        if (leaderboardText == null)
        {
            Debug.LogError("leaderboardText is not assigned in the Inspector!");
            return;
        }

        if (string.IsNullOrEmpty(data))
        {
            Debug.LogError("Leaderboard data is empty!");
            leaderboardText.text = "No scores found.";
            return;
        }

        string[] entries = data.Split('\n');
        leaderboardText.text = "Leaderboard:\n";

        foreach (string entry in entries)
        {
            if (!string.IsNullOrEmpty(entry))
            {
                string[] entryData = entry.Split('|');
                if (entryData.Length >= 2)
                {
                    string name = entryData[0];
                    string score = entryData[1];
                    leaderboardText.text += $"{name} - {score}\n";
                }
                else
                {
                    Debug.LogError("Invalid entry format: " + entry);
                }
            }
        }
    }
}
