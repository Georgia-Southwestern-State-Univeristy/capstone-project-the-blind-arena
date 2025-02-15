using System.Collections;
using UnityEngine;

// Water Boss Abilities
public class WaterGeyser : MonoBehaviour
{
    public GameObject geyserPrefab;
    public Transform[] geyserSpawns;
    public float attackDelay = 2f;
    public Animator animator;

    public void ActivateGeysers()
    {
        StartCoroutine(GeyserSequence());
    }

    private IEnumerator GeyserSequence()
    {
        if (animator != null)
        {
            animator.SetTrigger("Geyser");
        }
        yield return new WaitForSeconds(attackDelay);
        foreach (Transform spawn in geyserSpawns)
        {
            Instantiate(geyserPrefab, spawn.position, Quaternion.identity);
        }
    }
}

public class TidalWave : MonoBehaviour
{
    public GameObject wavePrefab;
    public Transform spawnPoint;
    public float attackDelay = 2f;
    public Animator animator;

    public void SummonWave()
    {
        StartCoroutine(WaveSequence());
    }

    private IEnumerator WaveSequence()
    {
        if (animator != null)
        {
            animator.SetTrigger("Wave");
        }
        yield return new WaitForSeconds(attackDelay);
        Instantiate(wavePrefab, spawnPoint.position, Quaternion.identity);
    }
}