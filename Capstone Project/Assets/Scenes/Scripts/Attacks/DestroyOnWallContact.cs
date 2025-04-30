using UnityEngine;

public class DestroyOnWallContact : MonoBehaviour
{
    private bool isTangible;

    void Start()
    {
        isTangible = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTangible && collision.collider.CompareTag("Wall"))
        {
            Destroy(transform.root.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTangible && other.CompareTag("Wall"))
        {
            Destroy(transform.root.gameObject);
        }
    }
}