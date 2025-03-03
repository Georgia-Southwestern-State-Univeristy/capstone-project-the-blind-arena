using System.Collections;
using UnityEngine;

// Lightning Boss Abilities
public class ThunderStrike : MonoBehaviour
{
    public GameObject lightningStrikePrefab;
    public Transform player;
    public float attackDelay = 1.5f;
    public Animator animator;

    public void Strike()
    {
        StartCoroutine(StrikeSequence());
    }

    private IEnumerator StrikeSequence()
    {
        if (animator != null)
        {
            animator.SetTrigger("Strike");
        }
        yield return new WaitForSeconds(attackDelay);
        Instantiate(lightningStrikePrefab, player.position, Quaternion.identity);
    }
}
