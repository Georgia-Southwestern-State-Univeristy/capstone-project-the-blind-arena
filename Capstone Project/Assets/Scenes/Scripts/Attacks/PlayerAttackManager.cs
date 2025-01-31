using System.Collections;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public GameObject player; // Reference to the player

    [System.Serializable]
    public class AttackAttributes
    {
        public string name;
        public float priority;
        public float damage;
        public float speed;
        public float duration; // Duration in seconds
        public int health;
        public bool detachFromPlayer; // Determines if attack stays or detaches
        public ColliderType colliderShape; // Shape of the attack collider
        public Vector3 colliderSize = Vector3.one; // Default size
        public Sprite attackSprite; // Assign sprite in Inspector
        public float gravityScale = 1.0f; // Control gravity effect in Inspector
        public Vector3 spriteSize = Vector3.one; // Control sprite size in Inspector
        public Vector3 spriteRotation = Vector3.zero; // Control sprite rotation in Inspector
    }

    public enum ColliderType { Box, Sphere, Capsule }
    
    public AttackAttributes[] attacks;

    public void TriggerAttack(string attackName)
    {
        AttackAttributes attack = System.Array.Find(attacks, a => a.name == attackName);
        if (attack != null)
            StartCoroutine(PerformAttack(attack));
        else
            Debug.LogError("Attack not defined: " + attackName);
    }

    private IEnumerator PerformAttack(AttackAttributes attack)
    {
        if (attack == null)
        {
            Debug.LogError("Attack attributes not found.");
            yield break;
        }

        GameObject attackObject = new GameObject(attack.name + "Collider");
        if (!attack.detachFromPlayer)
            attackObject.transform.SetParent(player.transform);
        
        attackObject.transform.position = player.transform.position;
        
        Collider collider = null;
        switch (attack.colliderShape)
        {
            case ColliderType.Box:
                BoxCollider box = attackObject.AddComponent<BoxCollider>();
                box.size = attack.colliderSize;
                collider = box;
                break;
            case ColliderType.Sphere:
                SphereCollider sphere = attackObject.AddComponent<SphereCollider>();
                sphere.radius = attack.colliderSize.x / 2;
                collider = sphere;
                break;
            case ColliderType.Capsule:
                CapsuleCollider capsule = attackObject.AddComponent<CapsuleCollider>();
                capsule.radius = attack.colliderSize.x / 2;
                capsule.height = attack.colliderSize.y;
                collider = capsule;
                break;
        }
        if (collider != null) collider.isTrigger = true;

        SpriteRenderer spriteRenderer = attackObject.AddComponent<SpriteRenderer>();
        if (attack.attackSprite != null)
        {
            spriteRenderer.sprite = attack.attackSprite;
            attackObject.transform.localScale = attack.spriteSize; // Apply sprite size
            attackObject.transform.eulerAngles = attack.spriteRotation; // Apply sprite rotation
        }
        else
            Debug.LogError("No sprite assigned to attack: " + attack.name);

        if (attack.detachFromPlayer)
        {
            Rigidbody rb = attackObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearVelocity = new Vector3(player.transform.forward.x * attack.speed, -attack.gravityScale, player.transform.forward.z * attack.speed);
        }
        
        yield return new WaitForSeconds(attack.duration);
        Destroy(attackObject);
    }
}
