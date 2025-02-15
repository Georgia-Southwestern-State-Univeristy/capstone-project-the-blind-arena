using System.Collections;
using UnityEngine;

public class WaterGeyser : MonoBehaviour
{
    public GameObject geyserPrefab;
    public Transform[] geyserSpawns;

    public void ActivateGeysers()
    {
        foreach (Transform spawn in geyserSpawns)
            Instantiate(geyserPrefab, spawn.position, Quaternion.identity);
    }
}

public class TidalWave : MonoBehaviour
{
    public GameObject wavePrefab;
    public Transform spawnPoint;

    public void SummonWave()
    {
        Instantiate(wavePrefab, spawnPoint.position, Quaternion.identity);
    }
}