using UnityEngine;

public class Ability : MonoBehaviour
{
    public string abilityName;
    public int damageAmount;

    // This method applies damage to the target
    public void ApplyDamage(Health target)
    {
        // Ensure the target has a Health component
        if (target != null)
        {
            target.Damage(damageAmount); // Call the TakeDamage method
            Debug.Log($"{abilityName} dealt {damageAmount} damage!");
        }
        else
        {
            Debug.LogError("Target does not have a Health component!");
        }
    }
}