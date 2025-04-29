using UnityEngine;

public class ProjectileCleaner : MonoBehaviour
{
    // Call this to destroy all active projectiles in the scene
    public static void DestroyAllProjectiles()
    {
        ProjectileAttack[] allProjectiles = FindObjectsOfType<ProjectileAttack>();

        foreach (ProjectileAttack projectile in allProjectiles)
        {
            Destroy(projectile.gameObject);
        }

        Debug.Log($"Destroyed {allProjectiles.Length} projectiles.");
    }
}