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
        public Vector3 colliderSize = Vector3.one;
        public Sprite attackSprite;
        public float delay;
        public Vector3 spriteSize = Vector3.one;
        public Vector3 spriteRotation = Vector3.zero;
        public AudioClip attackSound;
    }

    public AttackAttributes attack;

    public void TriggerAttack()
    {
        if (attack != null)
            StartCoroutine(PerformAttack(attack));
        else
            Debug.LogError("Enemy attack attributes not set!");
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
        if (!attack.detachFromEnemy) attackObject.transform.SetParent(enemy.transform);

        Collider collider = attackObject.AddComponent<BoxCollider>();
        ((BoxCollider)collider).size = attack.colliderSize;
        collider.isTrigger = true;

        AddSprite(attackObject, attack);
        return attackObject;
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
