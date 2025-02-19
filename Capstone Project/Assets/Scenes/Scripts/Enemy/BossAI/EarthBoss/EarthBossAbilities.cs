using System.Collections;
using UnityEngine;

// Earth Boss Abilities
public class SeismicSlam : MonoBehaviour
{
    public GameObject shockwavePrefab;
    public Transform spawnPoint;
    public float attackDelay = 2f;
    public Animator animator;

    public void Slam()
    {
        StartCoroutine(SlamSequence());
    }

    private IEnumerator SlamSequence()
    {
        if (animator != null)
        {
            animator.SetTrigger("Slam");
        }
        yield return new WaitForSeconds(attackDelay);
        Instantiate(shockwavePrefab, spawnPoint.position, Quaternion.identity);
    }
}

