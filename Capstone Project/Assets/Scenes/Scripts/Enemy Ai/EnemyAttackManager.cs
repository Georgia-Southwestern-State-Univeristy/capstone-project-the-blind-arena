using System.Collections;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    public GameObject enemy;
    public AudioSource audioSource;

    [System.Serializable]
    public class AttackAttributes
    {
        public string name;
        public float damage;
        public float speed;
        public float duration;
        public bool detachFromEnemy;
        public ColliderType colliderShape;
        public Vector3 colliderSize = Vector3.one;
        public Sprite attackSprite;
        public float delay;
        public Vector3 colliderRotation = Vector3.zero;
        public Vector3 spriteSize = Vector3.one;
        public Vector3 spriteRotation = Vector3.zero;
        public AudioClip attackSound;
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
        yield return new WaitForSeconds(attack.delay);

        PlaySound(attack.attackSound);
        GameObject attackObject = CreateAttackObject(attack);

        if (attack.detachFromEnemy)
        {
            Vector3 attackDirection = GetAttackDirection();
            LaunchAttack(attackObject, attackDirection, attack.speed);
        }

        yield return new WaitForSeconds(attack.duration);
        Destroy(attackObject);
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
        attackObject.transform.position = enemy.transform.position;
        
        if (!attack.detachFromEnemy) 
            attackObject.transform.SetParent(enemy.transform);

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
    }

    private Vector3 GetAttackDirection()
    {
        return enemy.transform.localScale.x > 0 ? Vector3.right : Vector3.left;
    }

    private void LaunchAttack(GameObject attackObject, Vector3 direction, float speed)
    {
        Rigidbody2D rb = attackObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearVelocity = direction * speed;
    }
}
