using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float gravityScale = 1f; // New variable to control gravity

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
        rb.useGravity = false; // Disable default gravity
    }

    void Update()
    {
        if (!playerRenderer.enabled)
        {
            Debug.Log("Player Renderer is disabled.");
        }

        // Movement input
        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.z = 2 * Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        Move();
        ApplyCustomGravity();
    }

    private void Move()
    {
        // Maintain Y velocity and apply movement in the XZ plane
        Vector3 velocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);
        rb.linearVelocity = velocity;

        CheckForFlipping();
    }

    private void ApplyCustomGravity()
    {
        // Apply custom gravity scaling
        rb.linearVelocity += new Vector3(0, Physics.gravity.y * gravityScale * Time.fixedDeltaTime, 0);
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
