using UnityEngine;

public class PlayerAttack : MonoBehaviour
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
        for (int i = 0; i < attackTypes.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                attackManager.TriggerAttack(attackTypes[i]);
            }
        }
    }
}
