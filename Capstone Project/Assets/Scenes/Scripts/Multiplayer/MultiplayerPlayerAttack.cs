using UnityEngine;
using Unity.Netcode; // Add this for Netcode

public class MultiplayerPlayerAttack : NetworkBehaviour
{
    public Animator animator;
    [Header("Attack Types")]
    public string[] attackTypes;

    [Header("Player Attack Manager")]
    public PlayerAttackManager attackManager;

    void Start()
    {
        if (!IsOwner)
            return;

        if (attackManager == null)
            attackManager = GetComponent<PlayerAttackManager>();

        if (attackTypes.Length == 0)
            Debug.LogWarning("No attack types assigned. Please assign them in the Inspector.");
    }

    void Update()
    {
        if (!IsOwner) return;

        if (attackTypes.Length > 0 && Input.GetMouseButtonDown(0)) // Left Click
            RequestAttackServerRpc(attackTypes[0]);

        if (attackTypes.Length > 1 && Input.GetMouseButtonDown(1)) // Right Click
            RequestAttackServerRpc(attackTypes[1]);

        if (attackTypes.Length > 2 && Input.GetKeyDown(KeyCode.E)) // E key
            RequestAttackServerRpc(attackTypes[2]);

        if (attackTypes.Length > 3 && Input.GetKeyDown(KeyCode.Q)) // Q key
            RequestAttackServerRpc(attackTypes[3]);
    }

    [ServerRpc]
    void RequestAttackServerRpc(string attackName)
    {
        if (!IsServer) return;

        attackManager.TriggerAttack(attackName);
    }

}
