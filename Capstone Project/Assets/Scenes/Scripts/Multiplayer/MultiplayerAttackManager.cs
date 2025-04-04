using System.Collections;
using UnityEngine;
using Unity.Netcode; // Add this for Netcode

public class MultplayerAttackManager : NetworkBehaviour
{
    public Health health;
    public GameObject player;
    public AudioSource audioSource;
    private PlayerController playerController;
    public Animator animator;

    public float damageModifier = 0f;

    [System.Serializable]
    public class AttackAttributes
    {
        public string name;
        public float damage, knockbackStrength, speed, duration, delay, gravityScale, lockDuration, staminaUse;
        public float cooldown; // Cooldown duration in seconds
        public bool detachFromPlayer, lockVelocity, isPhysical;
        public ColliderType colliderShape;
        public Vector3 colliderSize = Vector3.one, colliderRotation = Vector3.zero, spriteSize = Vector3.one, spriteRotation = Vector3.zero, startingOffset = Vector3.zero;
        public Sprite attackSprite;
        public AudioClip attackSound;
    }

    public enum ColliderType { Box, Sphere, Capsule }
    public AttackAttributes[] attacks;

    private bool isOnCooldown = false; // Tracks if the player is on cooldown
    private float cooldownTimer = 0f; // Tracks the remaining cooldown time in seconds

    private void Start() => playerController = player.GetComponent<PlayerController>();

    private void Update()
    {
        // Update the cooldown timer (in seconds)
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime; // Time.deltaTime is in seconds
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false; // Cooldown ended
            }
        }
    }

    public void TriggerAttack(string attackName)
    {
        if (!IsOwner) return;

        var attack = System.Array.Find(attacks, a => a.name == attackName);
        if (attack != null)
        {
            TriggerAttackServerRpc(attack.name);
            BroadcastAttackClientRpc(attackName); // Send to all clients
        }
        else
        {
            Debug.LogError($"Attack not defined: {attackName}");
        }
    }

    [ServerRpc]
    private void TriggerAttackServerRpc(string attackName)
    {
        PerformAttackClientRpc(attackName);
    }

    [ClientRpc]
    private void PerformAttackClientRpc(string attackName)
    {
        if (animator != null)
        {
            animator.SetTrigger(attackName);
        }
    }

    [ClientRpc]
    private void BroadcastAttackClientRpc(string attackName)
    {
        if (!IsOwner)
        {
            animator.SetTrigger(attackName);
        }
    }

    private IEnumerator PerformAttack(AttackAttributes attack)
    {
        // Check if the player has enough stamina to perform the attack
        if (!HasEnoughStamina(attack))
        {
            Debug.Log("Not enough stamina to perform the attack!");
            yield break;
        }

        // Flip the player based on mouse position
        FlipPlayerBasedOnMousePosition();

        // Start cooldown (in seconds)
        isOnCooldown = true;
        cooldownTimer = attack.cooldown; // Cooldown duration in seconds

        // Trigger the attack animation
        animator.SetTrigger(attack.name);

        // Lock player movement if the attack requires it
        if (attack.lockVelocity) playerController?.LockMovement(attack.lockDuration);

        // Wait for the attack delay (in seconds)
        yield return new WaitForSeconds(attack.delay);

        // Deduct stamina and proceed with the attack
        DeductStamina(attack.staminaUse);
        PlaySound(attack.attackSound);
        GameObject attackObject = CreateAttackObject(attack);

        // Launch the attack if it detaches from the player
        if (attack.detachFromPlayer) LaunchAttack(attackObject, GetAttackDirection(), attack.speed);

        // Wait for the attack duration (in seconds)
        yield return new WaitForSeconds(attack.duration);

        // Clean up the attack object
        if (attackObject) Destroy(attackObject);
    }

    private void FlipPlayerBasedOnMousePosition()
    {
        // Get the mouse position in world coordinates
        Vector3 mouseScreenPos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
        Plane groundPlane = new Plane(Vector3.up, player.transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance);

            // Determine if the mouse is to the left or right of the player
            if (mouseWorldPos.x < player.transform.position.x)
            {
                // Flip the player to face left
                player.transform.localScale = new Vector3(-Mathf.Abs(player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
            }
            else
            {
                // Flip the player to face right
                player.transform.localScale = new Vector3(Mathf.Abs(player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
            }
        }
    }

    private bool HasEnoughStamina(AttackAttributes attack) => health.stamina >= attack.staminaUse;
    private void DeductStamina(float amount) => health?.UseStamina(amount);
    private void PlaySound(AudioClip clip) { if (clip && audioSource) audioSource.PlayOneShot(clip); }

    private GameObject CreateAttackObject(AttackAttributes attack)
    {
        GameObject attackObject = new GameObject($"{attack.name}Collider");
        Vector3 offset = player.transform.localScale.x < 0 ? new Vector3(-attack.startingOffset.x, attack.startingOffset.y, attack.startingOffset.z) : attack.startingOffset;
        attackObject.transform.position = player.transform.position + offset;
        if (!attack.detachFromPlayer) attackObject.transform.SetParent(player.transform);

        GameObject colliderObject = new GameObject("Collider");
        colliderObject.transform.SetParent(attackObject.transform);
        colliderObject.transform.localPosition = Vector3.zero;
        colliderObject.transform.localEulerAngles = attack.colliderRotation;

        AddCollider(colliderObject, attack);
        AddSprite(attackObject, attack);

        attackObject.transform.localScale = new Vector3(
            player.transform.localScale.x > 0 ? attack.spriteSize.x : -attack.spriteSize.x,
            attack.spriteSize.y, attack.spriteSize.z);

        DamageOnHit damageOnHit = colliderObject.AddComponent<DamageOnHit>();
        damageOnHit.damageAmount = Mathf.RoundToInt(attack.damage + damageModifier);
        damageOnHit.knockbackStrength = attack.knockbackStrength;
        damageOnHit.detachFromPlayer = attack.detachFromPlayer;

        if (attack.isPhysical) SetupPhysicalObject(attackObject);
        else colliderObject.layer = LayerMask.NameToLayer("AttackObjects");

        Destroy(attackObject, attack.duration);
        return attackObject;
    }

    private void AddCollider(GameObject obj, AttackAttributes attack)
    {
        Collider collider = attack.colliderShape switch
        {
            ColliderType.Box => obj.AddComponent<BoxCollider>(),
            ColliderType.Sphere => obj.AddComponent<SphereCollider>(),
            ColliderType.Capsule => obj.AddComponent<CapsuleCollider>(),
            _ => null
        };
        if (collider)
        {
            collider.isTrigger = !attack.isPhysical;
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
        if (!attack.attackSprite) { Debug.LogWarning($"No sprite assigned to attack: {attack.name}"); return; }
        SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = attack.attackSprite;
        obj.transform.localScale = attack.spriteSize;
        obj.transform.eulerAngles = attack.spriteRotation;
    }

    private void SetupPhysicalObject(GameObject attackObject)
    {
        Rigidbody rb = attackObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        attackObject.layer = LayerMask.NameToLayer("AttackObjects");
    }

    private Vector3 GetAttackDirection()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
        Plane groundPlane = new Plane(Vector3.up, player.transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance);
            Vector3 direction = (mouseWorldPos - player.transform.position).normalized;
            direction.y = 0;
            return direction;
        }
        return player.transform.forward;
    }

    private void LaunchAttack(GameObject attackObject, Vector3 direction, float speed)
    {
        Rigidbody rb = attackObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rb.linearVelocity = direction * speed;
    }

    public void AdjustDamage(float damageChange, float duration)
    {
        StartCoroutine(AdjustDamageCoroutine(damageChange, duration));
    }

    private IEnumerator AdjustDamageCoroutine(float damageChange, float duration)
    {
        damageModifier += damageChange; // Increase damage
        yield return new WaitForSeconds(duration);
        damageModifier -= damageChange; // Revert damage after duration
    }




}