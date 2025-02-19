using System.Collections;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public Health health;
    public GameObject player;
    public AudioSource audioSource;
    private PlayerController playerController; // Reference to PlayerController

    [System.Serializable]
    public class AttackAttributes
    {
        public string name;
        public float priority;
        public float damage;
        public float speed;
        public float duration;
        public int staminaUse;
        public bool detachFromPlayer;
        public ColliderType colliderShape;
        public Vector3 colliderSize = Vector3.one;
        public Vector3 colliderRotation = Vector3.zero;
        public Sprite attackSprite;
        public float delay;
        public float gravityScale = 0;
        public Vector3 spriteSize = Vector3.one;
        public Vector3 spriteRotation = Vector3.zero;
        public AudioClip attackSound;
        
        public bool lockVelocity; // If true, locks velocity
        public float lockDuration; // How long to lock movement
        public bool isPhysical; // NEW: Makes the attack a solid object
        public Vector3 startingOffset = Vector3.zero;
    }

    public enum ColliderType { Box, Sphere, Capsule }
    public AttackAttributes[] attacks;

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>(); // Get reference to PlayerController
    }

    public void TriggerAttack(string attackName)
    {
        var attack = System.Array.Find(attacks, a => a.name == attackName);
        if (attack != null)
            StartCoroutine(PerformAttack(attack));
        else
            Debug.LogError($"Attack not defined: {attackName}");
    }

    private IEnumerator PerformAttack(AttackAttributes attack)
    {
        if (!HasEnoughStamina(attack)) yield break;

        if (attack.lockVelocity && playerController != null)
        {
            playerController.LockMovement(attack.lockDuration);
        }

        yield return new WaitForSeconds(attack.delay);

        DeductStamina(attack.staminaUse);
        PlaySound(attack.attackSound);
        GameObject attackObject = CreateAttackObject(attack);

        if (attack.detachFromPlayer)
        {
            Vector3 attackDirection = GetAttackDirection();
            LaunchAttack(attackObject, attackDirection, attack.speed);
        }

        yield return new WaitForSeconds(attack.duration);
        if (attackObject != null) Destroy(attackObject);

    }

    private bool HasEnoughStamina(AttackAttributes attack)
    {
        if (health != null && health.stamina < attack.staminaUse)
        {
            Debug.Log($"Not enough stamina for {attack.name}");
            return false;
        }
        return true;
    }

    private void DeductStamina(int amount)
    {
        health?.UseStamina(amount);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private GameObject CreateAttackObject(AttackAttributes attack)
{
    GameObject attackObject = new GameObject($"{attack.name}Collider");

    // Calculate the offset based on the player's facing direction
    Vector3 offset = attack.startingOffset;
    if (player.transform.localScale.x < 0) // If player is facing left
    {
        offset.x *= -1; // Flip the X offset
    }

    attackObject.transform.position = player.transform.position + offset;

    if (!attack.detachFromPlayer)
        attackObject.transform.SetParent(player.transform);

    // Create a separate GameObject for the collider
    GameObject colliderObject = new GameObject("Collider");
    colliderObject.transform.SetParent(attackObject.transform);
    colliderObject.transform.localPosition = Vector3.zero; // Ensure it stays aligned
    colliderObject.transform.localEulerAngles = attack.colliderRotation; // Apply rotation to just the collider

    AddCollider(colliderObject, attack, attack.isPhysical); // Attach collider to the new GameObject
    AddSprite(attackObject, attack); // Sprite remains on the main attack object

    attackObject.transform.localScale = new Vector3(
        player.transform.localScale.x > 0 ? attack.spriteSize.x : -attack.spriteSize.x,
        attack.spriteSize.y,
        attack.spriteSize.z
    );

    DamageOnHit damageOnHit = attackObject.AddComponent<DamageOnHit>();
    damageOnHit.damageAmount = Mathf.RoundToInt(attack.damage);

    if (attack.isPhysical)
    {
        Rigidbody rb = attackObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        attackObject.layer = LayerMask.NameToLayer("AttackObjects");
    }

    Destroy(attackObject, attack.duration);
    return attackObject;
}
    private void AddCollider(GameObject obj, AttackAttributes attack, bool isPhysical)
    {
        Collider collider = attack.colliderShape switch
        {
            ColliderType.Box => obj.AddComponent<BoxCollider>(),
            ColliderType.Sphere => obj.AddComponent<SphereCollider>(),
            ColliderType.Capsule => obj.AddComponent<CapsuleCollider>(),
            _ => null
        };

        if (collider != null)
        {
            collider.isTrigger = !isPhysical; // Only trigger if not physical
            SetColliderSize(collider, attack.colliderSize);
            obj.transform.eulerAngles = attack.colliderRotation;
        }
    }

    private void SetColliderSize(Collider collider, Vector3 size)
    {
        switch (collider)
        {
            case BoxCollider box: box.size = size; break;
            case SphereCollider sphere: sphere.radius = size.x / 2; break;
            case CapsuleCollider capsule:
                capsule.radius = size.x / 2;
                capsule.height = size.y;
                break;
        }
    }

    private void AddSprite(GameObject obj, AttackAttributes attack)
    {
        SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
        if (attack.attackSprite != null)
        {
            spriteRenderer.sprite = attack.attackSprite;
            obj.transform.localScale = attack.spriteSize;
            obj.transform.eulerAngles = attack.spriteRotation;
        }
        else
        {
            Debug.LogWarning($"No sprite assigned to attack: {attack.name}");
        }
    }

    private Vector3 GetAttackDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, player.transform.position.y);

        if (groundPlane.Raycast(ray, out float distance))
            return (ray.GetPoint(distance) - player.transform.position).normalized;

        return player.transform.forward;
    }

    private void LaunchAttack(GameObject attackObject, Vector3 direction, float speed)
    {
        Rigidbody rb = attackObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rb.linearVelocity = direction * speed;
    }
}
