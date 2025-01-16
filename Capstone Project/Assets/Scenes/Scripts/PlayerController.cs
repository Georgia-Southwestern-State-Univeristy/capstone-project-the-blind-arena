using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;

    private Rigidbody rb;
    private Vector3 moveDir;

    private Renderer playerRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRenderer = GetComponentInChildren<Renderer>(true);

        // Ensure Rigidbody settings are correct
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        if (!playerRenderer.enabled)
        {
            Debug.Log("Player Renderer is disabled.");
        }

        // Movement input
        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.z = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // Maintain Y velocity (gravity) and apply movement in XZ plane
        Vector3 velocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z) * speed;
        rb.linearVelocity = velocity; // Use rb.velocity for Rigidbody movement

        CheckForFlipping();
    }

    private void CheckForFlipping()
    {
        bool movingLeft = moveDir.x < 0;
        bool movingRight = moveDir.x > 0;

        if (movingLeft)
        {
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
        }
        if (movingRight)
        {
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
        }
    }
}
