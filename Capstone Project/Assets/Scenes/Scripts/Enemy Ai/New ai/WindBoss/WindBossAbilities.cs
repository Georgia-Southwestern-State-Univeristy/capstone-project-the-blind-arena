using System.Collections;
using UnityEngine;

public class TornadoSpin : MonoBehaviour
{
    public GameObject tornadoPrefab;
    public Transform tornadoSpawn;

    public void Spin()
    {
        Instantiate(tornadoPrefab, tornadoSpawn.position, Quaternion.identity);
    }
}

public class WindBlades : MonoBehaviour
{
    public GameObject windBladePrefab;
    public Transform[] bladeSpawns;

    public void ShootBlades()
    {
        foreach (Transform spawn in bladeSpawns)
            Instantiate(windBladePrefab, spawn.position, Quaternion.identity);
    }
}