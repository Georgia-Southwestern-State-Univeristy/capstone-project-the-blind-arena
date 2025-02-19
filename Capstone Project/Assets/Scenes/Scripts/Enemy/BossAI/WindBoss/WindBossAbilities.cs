using System.Collections;
using UnityEngine;

// Air Boss Abilities
public class TornadoSpin : MonoBehaviour
{
    public GameObject tornadoPrefab;
    public Transform tornadoSpawn;
    public float attackDelay = 2f;
    public Animator animator;

    public void Spin()
    {
        StartCoroutine(SpinSequence());
    }

    private IEnumerator SpinSequence()
    {
        if (animator != null)
        {
            animator.SetTrigger("Spin");
        }
        yield return new WaitForSeconds(attackDelay);
        Instantiate(tornadoPrefab, tornadoSpawn.position, Quaternion.identity);
    }
}

public class WindBlades : MonoBehaviour
{
    public GameObject windBladePrefab;
    public Transform[] bladeSpawns;
    public float attackDelay = 1.5f;
    public Animator animator;

    public void ShootBlades()
    {
        StartCoroutine(BladeSequence());
    }

    private IEnumerator BladeSequence()
    {
        if (animator != null)
        {
            animator.SetTrigger("Blade");
        }
        yield return new WaitForSeconds(attackDelay);
        foreach (Transform spawn in bladeSpawns)
        {
            Instantiate(windBladePrefab, spawn.position, Quaternion.identity);
        }
    }
}