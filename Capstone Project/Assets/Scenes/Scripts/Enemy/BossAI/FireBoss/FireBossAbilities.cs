using System.Collections;
using UnityEngine;

// Fire Boss Abilities
public class FlameDash : MonoBehaviour
{
    public float speed = 10f;
    public float attackDelay = 1.5f;
    public Animator animator;
    private Rigidbody2D rb;

    void Start() { rb = GetComponent<Rigidbody2D>(); }

    public void Dash(Vector2 direction)
    {
        StartCoroutine(DashSequence(direction));
    }

    private IEnumerator DashSequence(Vector2 direction)
    {
        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }
        yield return new WaitForSeconds(attackDelay);
        rb.linearVelocity = direction.normalized * speed;
        yield return new WaitForSeconds(1f);
        rb.linearVelocity = Vector2.zero;
    }
}

public class FirePillars : MonoBehaviour
{
    public GameObject[] firePillarPrefabs;
    public Transform[] firePillarSpawns;
    public float attackDelay = 2f;
    public Animator animator;

    public void Activate()
    {
        StartCoroutine(PillarSequence());
    }

    private IEnumerator PillarSequence()
    {
        if (animator != null)
        {
            animator.SetTrigger("Summon");
        }
        yield return new WaitForSeconds(attackDelay);
        foreach (Transform spawn in firePillarSpawns)
        {
            Instantiate(firePillarPrefabs[Random.Range(0, firePillarPrefabs.Length)], spawn.position, Quaternion.identity);
        }
    }
}