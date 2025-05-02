using System.Collections;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public Health health;
    public GameObject player;
    public AudioSource audioSource;
    private PlayerController playerController;
    public Animator animator;

    public float damageModifier = 0f;
    public bool playattacksound = true;

    [System.Serializable]
    public class AttackAttributes
    {
        public string name;
        public float damage, knockbackStrength, speed, duration, delay, gravityScale, lockDuration, staminaUse;
        public float cooldown;
        public float originalCooldown;
        public float originalStaminaUse;

        public float currentCooldown; // <- Per-attack cooldown tracker

        public bool detachFromPlayer, lockVelocity, isPhysical, isWall, isTangible;
        public ColliderType colliderShape;
        public Vector3 colliderSize = Vector3.one, colliderRotation = Vector3.zero, spriteSize = Vector3.one, spriteRotation = Vector3.zero, startingOffset = Vector3.zero;
        public Sprite attackSprite;
        public Animator attackAnimator;
        private GameObject[] targets;
    }

    public enum ColliderType { Box, Sphere, Capsule }
    public AttackAttributes[] attacks;

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();

        foreach (var attack in attacks)
        {
            attack.originalCooldown = attack.cooldown;
            attack.originalStaminaUse = attack.staminaUse;
            attack.currentCooldown = 0f; // Initialize cooldown timers
        }
    }

    private void Update()
    {
        playattacksound = true;

        // Update individual cooldowns per attack
        foreach (var attack in attacks)
        {
            if (attack.currentCooldown > 0f)
            {
                attack.currentCooldown -= Time.deltaTime;
                playattacksound = false;
            }
        }
    }

    public void TriggerAttack(string attackName)
    {
        var attack = System.Array.Find(attacks, a => a.name == attackName);
        if (attack == null)
        {
            Debug.LogError($"Attack not defined: {attackName}");
            return;
        }

        if (attack.currentCooldown > 0f)
        {
            Debug.Log($"{attack.name} is on cooldown!");
            return;
        }

        StartCoroutine(PerformAttack(attack));
    }

    private IEnumerator PerformAttack(AttackAttributes attack)
    {
        if (!HasEnoughStamina(attack))
        {
            Debug.Log("Not enough stamina to perform the attack!");
            yield break;
        }

        FlipPlayerBasedOnMousePosition();

        // Set this attack's cooldown
        attack.currentCooldown = attack.cooldown;

        animator.SetTrigger(attack.name);

        if (attack.lockVelocity)
            playerController?.LockMovement(attack.lockDuration);

        yield return new WaitForSeconds(attack.delay);

        DeductStamina(attack.staminaUse);
        GameObject attackObject = CreateAttackObject(attack);

        if (attack.detachFromPlayer)
            LaunchAttack(attackObject, GetAttackDirection(), attack.speed);

        yield return new WaitForSeconds(attack.duration);

        if (attackObject)
            Destroy(attackObject);
    }

    private AttackAttributes GetAttackAttributesByType(string attackType)
    {
        foreach (var attack in attacks)
        {
            if (attack.name == attackType)
            {
                return attack;
            }
        }

        Debug.LogWarning($"Attack with type '{attackType}' not found.");
        return null;
    }

    public bool CanUseAttack(string attackType)
    {
        var attack = GetAttackAttributesByType(attackType);
        return HasEnoughStamina(attack);
    }

    private void FlipPlayerBasedOnMousePosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);
        Plane groundPlane = new Plane(Vector3.up, player.transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance);
            if (mouseWorldPos.x < player.transform.position.x)
            {
                player.transform.localScale = new Vector3(-Mathf.Abs(player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
            }
            else
            {
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
        AddAnimator(attackObject, attack);

        attackObject.transform.localScale = new Vector3(
            player.transform.localScale.x > 0 ? attack.spriteSize.x : -attack.spriteSize.x,
            attack.spriteSize.y, attack.spriteSize.z);

        DamageOnHit damageOnHit = colliderObject.AddComponent<DamageOnHit>();
        damageOnHit.damageAmount = Mathf.RoundToInt(attack.damage + damageModifier);
        damageOnHit.knockbackStrength = attack.knockbackStrength;
        damageOnHit.detachFromPlayer = attack.detachFromPlayer;

        if (attack.isWall)
        {
            attackObject.tag = "Wall";
            colliderObject.tag = "Wall";
        }

        if (attack.isPhysical)
            SetupPhysicalObject(attackObject);
        //else
        //    colliderObject.layer = LayerMask.NameToLayer("AttackObjects");

        if (attack.isTangible)
        {
            attackObject.AddComponent<DestroyOnWallContact>();
        }
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
        if (!attack.attackSprite)
        {
            Debug.LogWarning($"No sprite assigned to attack: {attack.name}");
            return;
        }

        SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = attack.attackSprite;
        obj.transform.localScale = attack.spriteSize;
        obj.transform.eulerAngles = attack.spriteRotation;
    }

    private void AddAnimator(GameObject obj, AttackAttributes attack)
    {
        if (!attack.attackAnimator)
        {
            Debug.LogWarning($"No animator assigned to attack: {attack.name}");
            return;
        }

        Animator animator = obj.AddComponent<Animator>();
        attack.attackAnimator = animator;
    }

    private void SetupPhysicalObject(GameObject attackObject)
    {
        Rigidbody rb = attackObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        //attackObject.layer = LayerMask.NameToLayer("AttackObjects");
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
        Debug.Log($"attackObject is {(attackObject == null ? "null" : "not null")}");
        Debug.Log($"Rigidbody is {attackObject?.GetComponent<Rigidbody>()}");
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
        damageModifier += damageChange;
        yield return new WaitForSeconds(duration);
        damageModifier -= damageChange;
    }

    public void ResetCooldowns()
    {
        foreach (var attack in attacks)
        {
            attack.cooldown = attack.originalCooldown;
            attack.staminaUse = attack.originalStaminaUse;
            attack.currentCooldown = 0f; // Reset cooldown timer
        }
    }
}
