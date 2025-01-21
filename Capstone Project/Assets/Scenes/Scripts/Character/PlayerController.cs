using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float gravityScale = 5f; // New variable to control gravity

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
    

        // Movement input with a threshold to avoid drift
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Set a threshold to eliminate tiny input values
        float inputThreshold = 0.01f;
        moveDir.x = Mathf.Abs(horizontalInput) > inputThreshold ? horizontalInput : 0;
        moveDir.z = Mathf.Abs(verticalInput) > inputThreshold ? 2 * verticalInput : 0;
    }


    private void FixedUpdate()
    {
        Move();
        ApplyCustomGravity();
    }

    private void Move()
    {
        // Check if there is no movement input
        if (moveDir.x == 0 && moveDir.z == 0)
        {
            // Stop the character's velocity
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            // Maintain Y velocity and apply movement in the XZ plane
            Vector3 velocity = new Vector3(moveDir.x * speed, 0 , moveDir.z * speed);
            rb.linearVelocity = velocity;
        }

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
            // Flip the player without affecting the Y or Z scale
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        if (movingRight)
        {
            // Ensure the player faces right without affecting the Y or Z scale
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}

