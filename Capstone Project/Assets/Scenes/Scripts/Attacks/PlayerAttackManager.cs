using System.Collections;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    // Reference to the Player GameObject
    public GameObject player;

    // Attack attributes structure
    [System.Serializable]
    public class AttackAttributes
    {
        public string name;
        public float priority;
        public float damage;
        public float speed;
        public int health;
    }

    // List of attacks (can be configured in the Inspector)
    public AttackAttributes[] attacks;

    // Method to trigger an attack
    public void TriggerAttack(string attackName)
    {
        switch (attackName)
        {
            case "BasicAttack":
                StartCoroutine(BasicAttack(System.Array.Find(attacks, a => a.name == "BasicAttack")));
                break;

            // Add more cases here for other attacks

            default:
                Debug.LogError("Attack not defined: " + attackName);
                break;
        }
    }

    // Coroutine for an attack
    private IEnumerator BasicAttack(AttackAttributes attack)
    {
        if (attack == null)
        {
            Debug.LogError("Attack attributes not found.");
            yield break;
        }

        // Create a new GameObject for the attack collider
        GameObject attackCollider = new GameObject(attack.name + "Collider");

        // Set the attackCollider as a child of the player
        attackCollider.transform.SetParent(player.transform);

        // Align the attackCollider to the player's position
        attackCollider.transform.localPosition = Vector3.zero;

        // Add a BoxCollider component
        BoxCollider boxCollider = attackCollider.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        // Add a MeshRenderer to display the texture
        MeshRenderer meshRenderer = attackCollider.AddComponent<MeshRenderer>();

        // Load the texture directly from the Resources folder
        Texture2D texture = Resources.Load<Texture2D>("Sprites/Basic_Rock"); // Path relative to Resources folder
        if (texture == null)
        {
            Debug.LogError("Basic_Rock.png not found! Ensure it's in the Resources folder.");
        }
        else
        {
            // Create a new material and assign the texture
            Material attackMaterial = new Material(Shader.Find("Standard")); // Changed variable name to attackMaterial
            attackMaterial.mainTexture = texture;
            meshRenderer.material = attackMaterial;
        }



        // Assign a material with a simple color
        Material material = new Material(Shader.Find("Standard"));
        material.color = Color.red; // Use a red color for the placeholder
        meshRenderer.material = material;


        // Adjust the collider size (customize as needed)
        boxCollider.size = new Vector3(1, 1, 1);

        // Simulate attack speed (time before the attack becomes inactive)
        yield return new WaitForSeconds(attack.speed);

        // Reduce health of the attack on interaction (can be expanded for interaction logic)
        attack.health -= 1;

        if (attack.health <= 0)
        {
            Destroy(attackCollider);
        }
        else
        {
            Destroy(attackCollider, attack.speed);
        }
    }
}
