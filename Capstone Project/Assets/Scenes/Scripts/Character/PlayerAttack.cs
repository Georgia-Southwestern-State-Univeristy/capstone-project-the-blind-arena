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

        for (int i = 0; i < attackTypes.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                // Tell server to trigger the attack
                TriggerAttackServerRpc(attackTypes[i]);
            }
        }
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
