using UnityEngine;

public class RestHider : MonoBehaviour
{
    public GameObject Object;
    public int round;
    public double deathcounter => GameData.deathcounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Object = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (deathcounter > round)
            Destroy(Object);
    }
}
