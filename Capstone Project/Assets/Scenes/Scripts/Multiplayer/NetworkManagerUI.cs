using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button mainMenuButton; // Reference to main menu button
    [SerializeField] private Button startBossButton; // Start boss fight button
    [SerializeField] private GameObject boss; // Reference to boss GameObject

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StartHost();
            }
        });

        clientButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StartClient();
            }
        });

        mainMenuButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
                {
                    NetworkManager.Singleton.Shutdown(); // Stop hosting or disconnect client
                }

                if (NetworkManager.Singleton.gameObject != null)
                {
                    Destroy(NetworkManager.Singleton.gameObject); // Destroy NetworkManager if it persists
                }
            }

            SceneManager.LoadScene(0); // Load main menu scene

        });

        startBossButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsServer) // Ensure only the host activates the boss
            {
                boss.SetActive(true); // Activate the boss
                BossActivationClientRpc(); // Notify clients
            }
        });

        boss.SetActive(false); // Ensure boss is disabled at the start
    }

    [ClientRpc]
    private void BossActivationClientRpc()
    {
        if (!NetworkManager.Singleton.IsServer) // Clients only
        {
            boss.SetActive(true);
        }
    }
}

