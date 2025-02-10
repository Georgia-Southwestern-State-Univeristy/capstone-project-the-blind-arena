using System.Collections;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public Health health;
    public GameObject player;
    public AudioSource audioSource;  // Audio source for playing attack sounds

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
        public AudioClip attackSound;  // Sound effect for this attack
    }

    public enum ColliderType { Box, Sphere, Capsule }
    public AttackAttributes[] attacks;

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
        Destroy(attackObject);
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
        attackObject.transform.position = player.transform.position;

        if (!attack.detachFromPlayer)
            attackObject.transform.SetParent(player.transform);

        AddCollider(attackObject, attack);
        AddSprite(attackObject, attack);

        // Add and configure the DamageOnHit component
        DamageOnHit damageOnHit = attackObject.AddComponent<DamageOnHit>();
        damageOnHit.damageAmount = Mathf.RoundToInt(attack.damage);  // Ensure damage is set properly

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

        if (collider != null)
        {
            collider.isTrigger = true;
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