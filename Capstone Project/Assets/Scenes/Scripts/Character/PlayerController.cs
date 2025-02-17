using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = .1f;
    [SerializeField] private float gravityScale = 20f;

    public Animator animator;
    private Rigidbody rb;
    private Vector3 moveDir;
    private bool isMovementLocked = false; // NEW: Track movement lock state
    private Renderer playerRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRenderer = GetComponentInChildren<Renderer>(true);

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.useGravity = false;
    }

    void Update()
    {
        if (isMovementLocked)
        {
            moveDir = Vector3.zero;
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float combinedInput = (horizontalInput * horizontalInput) + (verticalInput * verticalInput);

        float inputThreshold = 0.01f;
        moveDir.x = Mathf.Abs(horizontalInput) > inputThreshold ? horizontalInput : 0;
        moveDir.z = Mathf.Abs(verticalInput) > inputThreshold ? verticalInput : 0;

        animator.SetFloat("Speed", Mathf.Abs(combinedInput));
    }

    private void FixedUpdate()
    {
        if (!isMovementLocked)
        {
            Move();
        }
        ApplyCustomGravity();
    }

    private void Move()
    {
        if (moveDir.x == 0 && moveDir.z == 0)
        {
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            Vector3 input = new Vector3(moveDir.x, 0, moveDir.z).normalized;
            Vector3 velocity = input * speed;
            rb.linearVelocity = new Vector3((float)(1.5 * velocity.x), 0, 2 * velocity.z);
        }

        CheckForFlipping();
    }

    private void ApplyCustomGravity()
    {
        rb.linearVelocity += new Vector3(0, Physics.gravity.y * gravityScale * Time.fixedDeltaTime, 0);
    }

    private void CheckForFlipping()
    {
        if (moveDir.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveDir.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void LockMovement(float duration)
    {
        if (!isMovementLocked)
        {
            isMovementLocked = true;
            rb.linearVelocity = Vector3.zero;
            Invoke(nameof(UnlockMovement), duration);
        }
    }

    private void UnlockMovement()
    {
        isMovementLocked = false;
    }
}
