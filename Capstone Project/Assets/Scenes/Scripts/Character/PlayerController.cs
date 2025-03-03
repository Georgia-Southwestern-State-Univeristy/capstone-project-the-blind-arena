using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float dashSpeedMultiplier = 2f; // Multiplier for dash speed
    [SerializeField] private float gravityScale = 20f;
    [SerializeField] private float dashStaminaCost = 0.3f; // Stamina cost per frame while dashing
    private float fixedHeight = 0.6f;
    private bool isDashing = false;

    public Animator animator;
    private Rigidbody rb;
    private Vector3 moveDir;
    private bool isMovementLocked = false;
    private Vector3 externalForce = Vector3.zero; // Tracks enemy knockback or external forces
    private Renderer playerRenderer;
    private Health healthScript;
    private float originalSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRenderer = GetComponentInChildren<Renderer>(true);
        healthScript = GetComponent<Health>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.useGravity = false;

        originalSpeed = speed;
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
        animator.SetBool("isDashing", isDashing);

        HandleDashInput();
    }

    private void FixedUpdate()
    {
        ApplyCustomGravity();
        Move();
    }

    private void Move()
    {
        if (isMovementLocked)
        {
            Vector3 lockedExternalForce = new Vector3(externalForce.x, 0, externalForce.z);
            rb.linearVelocity = lockedExternalForce;
            return;
        }

        Vector3 inputVelocity = Vector3.zero;
        if (moveDir.x != 0 || moveDir.z != 0)
        {
            Vector3 input = new Vector3(moveDir.x, 0, moveDir.z).normalized;
            inputVelocity = new Vector3(1.5f * input.x, 0, 2f * input.z) * speed;
        }

        Vector3 combinedVelocity = inputVelocity + new Vector3(externalForce.x, 0, externalForce.z);
        rb.linearVelocity = combinedVelocity;

        Vector3 position = transform.position;
        position.y = fixedHeight;
        transform.position = position;

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

    public void ApplyExternalForce(Vector3 force, float duration)
    {
        externalForce = new Vector3(force.x, 0, force.z);
        Invoke(nameof(ClearExternalForce), duration);
    }

    private void ClearExternalForce()
    {
        externalForce = Vector3.zero;
    }

    private void HandleDashInput()
    {
        if (Input.GetKey(KeyCode.Space) && healthScript.stamina > 0)
        {
            if (!isDashing)
            {
                isDashing = true;
                speed *= dashSpeedMultiplier;
            }

            healthScript.UseStamina(dashStaminaCost);
        }
        else if (isDashing)
        {
            isDashing = false;
            speed = originalSpeed;
        }
    }
}
