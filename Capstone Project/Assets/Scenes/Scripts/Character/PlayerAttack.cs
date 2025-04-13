using UnityEngine;
using Unity.Netcode;

public class PlayerAttack : NetworkBehaviour
{
    public Animator animator;

    [Header("Attack Types")]
    public string[] attackTypes; // Should be max 4

    [Header("Player Attack Manager")]
    public PlayerAttackManager attackManager;

    void Start()
    {
        if (attackManager == null)
            attackManager = GetComponent<PlayerAttackManager>();

        if (attackTypes.Length == 0)
            Debug.LogWarning("No attack types assigned. Please assign them in the Inspector.");
    }

    void Update()
    {
        if (!IsOwner) return;

        if (attackTypes.Length > 0 && Input.GetMouseButtonDown(0)) // Left Click
            TriggerAttackServerRpc(attackTypes[0]);

        if (attackTypes.Length > 1 && Input.GetMouseButtonDown(1)) // Right Click
            TriggerAttackServerRpc(attackTypes[1]);

        if (attackTypes.Length > 2 && Input.GetKeyDown(KeyCode.E)) // E key
            TriggerAttackServerRpc(attackTypes[2]);

        if (attackTypes.Length > 3 && Input.GetKeyDown(KeyCode.Q)) // Q key
            TriggerAttackServerRpc(attackTypes[3]);
    }

    [ServerRpc]
    void TriggerAttackServerRpc(string attackType)
    {
        // Broadcast to all clients (including host) to play attack
        TriggerAttackClientRpc(attackType);
    }

    [ClientRpc]
    void TriggerAttackClientRpc(string attackType)
    {
        if (attackManager != null)
        {
            attackManager.TriggerAttack(attackType);
        }
    }
}
