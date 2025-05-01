using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WindWall : MonoBehaviour
{
    [Header("Push Settings")]
    public Vector3 pushDirection = Vector3.right; // Direction to push objects
    public float maxPushForce = 20f; // Maximum force applied when player is farthest from the push edge

    private Collider zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null)
            return;

        // Get zone bounds and local position of the player
        Vector3 localPos = transform.InverseTransformPoint(other.transform.position);
        Vector3 localDirection = transform.InverseTransformDirection(pushDirection.normalized);

        // Find the axis we're pushing along (X, Y, or Z)
        int axis = GetPrimaryAxis(localDirection);

        // Calculate the bounds size on that axis
        float halfExtent = zoneCollider.bounds.extents[axis];
        float centerOffset = localPos[axis];

        // Normalize distance from push origin edge (range: 0 to 1)
        float distanceRatio = Mathf.InverseLerp(-halfExtent, halfExtent, centerOffset);
        if (localDirection[axis] < 0) distanceRatio = 1f - distanceRatio;

        // Apply force proportional to how far player is from the push edge
        float forceMagnitude = -distanceRatio * maxPushForce;
        rb.AddForce(pushDirection.normalized * forceMagnitude, ForceMode.Force);
    }

    private int GetPrimaryAxis(Vector3 direction)
    {
        direction = new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z));
        if (direction.x > direction.y && direction.x > direction.z) return 0;
        if (direction.y > direction.z) return 1;
        return 2;
    }
}