using System.Collections;
using UnityEngine;

// Fire Boss Abilities
public class FlameDash : MonoBehaviour
{
    public float speed = 10f;
    public float attackDelay = 1.5f;
    public Animator animator;
    private Rigidbody2D rb;

    void Start() => rb = GetComponent<Rigidbody2D>();

    public void Dash(Vector2 direction)
    {
        StartCoroutine(DashSequence(direction));
    }

    private IEnumerator DashSequence(Vector2 direction)
    {
        animator?.SetTrigger("Dash");  // Null-conditional operator simplifies null checks
        yield return new WaitForSeconds(attackDelay);

        rb.linearVelocity = direction.normalized * speed;  // Use 'velocity' instead of 'linearVelocity' for Rigidbody2D
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
        animator?.SetTrigger("Summon");  // Null-conditional operator simplifies null checks
        yield return new WaitForSeconds(attackDelay);

        foreach (var spawn in firePillarSpawns)  // Use 'var' for brevity
        {
            int randomIndex = Random.Range(0, firePillarPrefabs.Length);
            Instantiate(firePillarPrefabs[randomIndex], spawn.position, Quaternion.identity);
        }
    }
}
