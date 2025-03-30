using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSceneTrigger : NetworkBehaviour
{
    [SerializeField] private int sceneIndex = 3; // Set this to the scene index you want
    [SerializeField] private string triggerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Only the server should handle scene loading
        if (!other.CompareTag(triggerTag)) return;

        LoadSceneForAllClients(sceneIndex);
    }

    private void LoadSceneForAllClients(int sceneIndex)
    {
        if (NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                SceneManager.GetSceneByBuildIndex(sceneIndex).name,
                LoadSceneMode.Single
            );
        }
        else
        {
            Debug.LogError("SceneManager is null. Ensure Netcode for GameObjects is set up correctly.");
        }
    }
}
