using UnityEngine;
using Unity.Cinemachine;

public class BossCameraSwitcher : MonoBehaviour
{
    public CinemachineCamera playerCamera;
    public CinemachineCamera targetCamera;
    public CinemachineCamera enemyCamera;
    public Transform boss;
    public Transform player;
    public float switchDistance = 20f;
    public bool focusOnBoss = false;

    private int activePriority = 10;
    private int inactivePriority = 5;

    void Update()
    {
        if (boss == null || player == null)
            return;
        
        // Calculate the distance between the boss and the player.
        float distance = Vector3.Distance(boss.position, player.position);

        //
        if (focusOnBoss)
        {
            playerCamera.Priority = inactivePriority;
            targetCamera.Priority = inactivePriority;
            enemyCamera.Priority = activePriority;
        }

        // When the boss is close enough, switch to the TargetsCam.
        else if (distance <= switchDistance)
        {
            playerCamera.Priority = inactivePriority;
            targetCamera.Priority = activePriority;
            enemyCamera.Priority = inactivePriority;
        }
        else // Otherwise, use the general PlayerCam.
        {
            playerCamera.Priority = activePriority;
            targetCamera.Priority = inactivePriority;
            enemyCamera.Priority = inactivePriority;
        }
    }
}