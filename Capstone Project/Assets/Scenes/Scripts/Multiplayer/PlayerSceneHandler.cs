using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerSceneHandler : NetworkBehaviour
{
    private bool sceneStart = false;
    private Camera playerCamera;
    private MultiplayerPlayerController playerController;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        if (!sceneStart)
        {
            SceneManager.sceneLoaded += OnSceneStart;
        }
    }

    private void OnSceneStart(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene" && !sceneStart)
        {
            playerCamera = GameObject.Find("CameraOne")?.GetComponent<Camera>();
            playerController = GetComponent<MultiplayerPlayerController>();

            sceneStart = true;
        }
    }
}
