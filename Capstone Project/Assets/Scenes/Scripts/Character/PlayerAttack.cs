using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // List of attack names that can be assigned in Unity Inspector
    [Header("Attack Types")]
    public string[] attackTypes;

    // Reference to the PlayerAttackManager script
    [Header("Player Attack Manager")]
    public PlayerAttackManager attackManager;  // This will be assigned in the Inspector

    void Start()
    {
        // Automatically find PlayerAttackManager on the same GameObject
        if (attackManager == null)
        {
            attackManager = GetComponent<PlayerAttackManager>();
            if (attackManager == null)
            {
                Debug.LogError("PlayerAttackManager is not assigned or not found on the same GameObject!");
            }
        }

        // Ensure attackTypes is not empty
        if (attackTypes.Length == 0)
        {
            Debug.LogWarning("No attack types assigned. Please assign them in the Inspector.");
        }
    }



    void Update()
    {
        // Loop through the attackTypes array and check input
        for (int i = 0; i < attackTypes.Length; i++)
        {
            // Check if the key corresponding to the attack is pressed
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                // Trigger the attack based on index in attackTypes array
                attackManager.TriggerAttack(attackTypes[i]);
                Debug.LogWarning("Attack"+ i);
            }
        }
    }
}
