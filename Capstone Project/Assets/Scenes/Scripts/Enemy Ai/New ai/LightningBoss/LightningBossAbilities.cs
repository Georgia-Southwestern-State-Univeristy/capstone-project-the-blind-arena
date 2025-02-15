using System.Collections;
using UnityEngine;

public class ThunderStrike : MonoBehaviour
{
    public GameObject lightningStrikePrefab;
    public Transform player;

    public void Strike()
    {
        Vector2 targetPos = player.position;
        Instantiate(lightningStrikePrefab, targetPos, Quaternion.identity);
    }
}

public class ChainLightning : MonoBehaviour
{
    public GameObject lightningBoltPrefab;
    public Transform[] chainTargets;

    public void CastChainLightning()
    {
        foreach (Transform target in chainTargets)
            Instantiate(lightningBoltPrefab, target.position, Quaternion.identity);
    }
}
