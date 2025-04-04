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
        if (!IsOwner)
            return;

        for (int i = 0; i < attackTypes.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                RequestAttackServerRpc(attackTypes[i]);
            }
        }
    }

    [ServerRpc]
    void RequestAttackServerRpc(string attackName)
    {
        if (!IsServer) return;

        attackManager.TriggerAttack(attackName);
    }

}
