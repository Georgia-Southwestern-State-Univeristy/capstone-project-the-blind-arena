using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneController : NetworkBehaviour
{
    public static NetworkSceneController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TransitionToScene(string sceneName)
    {
        if (!IsServer) return; // Only the server/host can change scenes

        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
