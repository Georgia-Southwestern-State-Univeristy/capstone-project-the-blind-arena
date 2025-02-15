using System.Collections;
using UnityEngine;

public class SeismicSlam : MonoBehaviour
{
    public GameObject shockwavePrefab;
    public Transform spawnPoint;

    public void Slam()
    {
        Instantiate(shockwavePrefab, spawnPoint.position, Quaternion.identity);
    }
}

public class RockBarricade : MonoBehaviour
{
    public GameObject rockWallPrefab;
    public Transform[] wallSpawns;

    public void CreateWalls()
    {
        foreach (Transform spawn in wallSpawns)
            Instantiate(rockWallPrefab, spawn.position, Quaternion.identity);
    }
}
