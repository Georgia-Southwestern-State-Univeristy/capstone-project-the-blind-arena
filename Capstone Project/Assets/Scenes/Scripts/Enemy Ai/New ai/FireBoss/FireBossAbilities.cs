using System.Collections;
using UnityEngine;

// Fire Boss Abilities
public class FlameDash : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;

    void Start() { rb = GetComponent<Rigidbody2D>(); }

    public void Dash(Vector3 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        StartCoroutine(ResetVelocity());
    }

    IEnumerator ResetVelocity()
    {
        yield return new WaitForSeconds(1f);
        rb.linearVelocity = Vector3.zero;
    }
}

public class FirePillars : MonoBehaviour
{
    public GameObject firePillarPrefab;
    public Transform[] firePillarSpawns;

    public void Activate()
    {
        foreach (Transform spawn in firePillarSpawns)
            Instantiate(firePillarPrefab, spawn.position, Quaternion.identity);
    }
}

public class MeteorShower : MonoBehaviour
{
    public GameObject meteorPrefab;
    public Transform[] meteorSpawns;

    public void RainMeteors()
    {
        foreach (Transform spawn in meteorSpawns)
            Instantiate(meteorPrefab, spawn.position, Quaternion.identity);
    }
}