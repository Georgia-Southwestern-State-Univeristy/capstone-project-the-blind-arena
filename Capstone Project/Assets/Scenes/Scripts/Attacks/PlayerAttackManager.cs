using System.Collections;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public Health health;
    public GameObject player;
    public AudioSource audioSource;
    private PlayerController playerController;

    [System.Serializable]
    public class AttackAttributes
    {
        public string name;
        public float priority, damage, knockbackStrength, speed, duration, delay, gravityScale, lockDuration;
        public int staminaUse;
        public bool detachFromPlayer, lockVelocity, isPhysical;
        public ColliderType colliderShape;
        public Vector3 colliderSize = Vector3.one, colliderRotation = Vector3.zero, spriteSize = Vector3.one, spriteRotation = Vector3.zero, startingOffset = Vector3.zero;
        public Sprite attackSprite;
        public AudioClip attackSound;
    }

    public enum ColliderType { Box, Sphere, Capsule }
    public AttackAttributes[] attacks;

    private void Start() => playerController = player.GetComponent<PlayerController>();

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
        if (attack.lockVelocity) playerController?.LockMovement(attack.lockDuration);
        yield return new WaitForSeconds(attack.delay);

        DeductStamina(attack.staminaUse);
        PlaySound(attack.attackSound);
        GameObject attackObject = CreateAttackObject(attack);

        if (attack.detachFromPlayer) LaunchAttack(attackObject, GetAttackDirection(), attack.speed);
        yield return new WaitForSeconds(attack.duration);
        if (attackObject) Destroy(attackObject);
    }

    private bool HasEnoughStamina(AttackAttributes attack) => health == null || health.stamina >= attack.staminaUse;
    private void DeductStamina(int amount) => health?.UseStamina(amount);
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
        damageOnHit.damageAmount = Mathf.RoundToInt(attack.damage);
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return new Plane(Vector3.up, player.transform.position.y).Raycast(ray, out float distance)
            ? (ray.GetPoint(distance) - player.transform.position).normalized
            : player.transform.forward;
    }

    private void LaunchAttack(GameObject attackObject, Vector3 direction, float speed)
    {
        Rigidbody rb = attackObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rb.linearVelocity = direction * speed;
    }
}