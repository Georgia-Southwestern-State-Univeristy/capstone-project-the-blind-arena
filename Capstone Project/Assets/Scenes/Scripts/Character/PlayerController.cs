using System.Collections;
using UnityEngine;
using Unity.Netcode; // Add this for Netcode

public class PlayerController : NetworkBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private float dashSpeedMultiplier = 2f; // Multiplier for dash speed
    [SerializeField] private float gravityScale = 20f;
    [SerializeField] private float dashStaminaCost; // Stamina cost per frame while dashing
    public Animator animator;
    private float fixedHeight = 0.6f;
    private bool isDashing = false;
    private Rigidbody rb;
    private Vector3 moveDir;
    private bool isMovementLocked = false;
    private Vector3 externalForce = Vector3.zero; // Tracks enemy knockback or external forces
    private Renderer playerRenderer;
    private Health healthScript;
    public float originalSpeed;

    public double deathcounter => GameData.deathcounter;
    public double tutorialcounter = 0;

    [SerializeField] private GameObject objectForPlayerMovement;
    [SerializeField] private GameObject enemyAI;
    [SerializeField] private GameObject enemyAI2;
    [SerializeField] private GameObject enemyAI3;
    [SerializeField] private GameObject enemyAI4;
    [SerializeField] private GameObject enemyAI5;


    private bool enemyAI2Activated = false;
    private bool enemyAI3Activated = false;
    private bool enemyAI4Activated = false;
    private bool enemyAI5Activated = false;

    public Camera playerCamera; // Assign this in the Inspector or instantiate dynamically

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

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; // Only allow the owner to process input

        // Enable necessary components for the local player
        GetComponent<PlayerController>().enabled = true;

        if (!IsOwner)
        {
            enabled = false; // Disable the script for non-owners
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false); // Disable camera for non-owners
            }
            return;
        }

        // If this is the local player, make the camera follow them
        if (playerCamera == null)
        {
            playerCamera = Camera.main; // Find the main camera if not assigned
        }

        if (playerCamera != null)
        {
            playerCamera.GetComponent<CameraFollow>().SetTarget(transform);
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Ensure only the owner processes input

        if (isMovementLocked)
        {
            moveDir = Vector3.zero;
            return;
        }
        
        if (deathcounter == 1)
        {
            
            enemyAI.SetActive(true);
        }

        else if (GameData.deathcounter == 2 && !enemyAI2Activated)
        {
            enemyAI2Activated = true; // Prevents repeated activation
            StartCoroutine(ActivateEnemyAI2Delayed());
        }

        else if (GameData.deathcounter == 3 && !enemyAI3Activated)
        {
            enemyAI3Activated = true; 
            StartCoroutine(ActivateEnemyAI3Delayed());
        }

        else if (GameData.deathcounter == 4 && !enemyAI4Activated)
        {
            enemyAI4Activated = true; 
            StartCoroutine(ActivateEnemyAI4Delayed());
        }

        else if (GameData.deathcounter == 5 && !enemyAI5Activated)
        {
            enemyAI5Activated = true; 
            StartCoroutine(ActivateEnemyAI5Delayed());
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
        if (!IsOwner) return; // Ensure only the owner processes physics

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

    private IEnumerator ActivateEnemyAIDelayed()
    {
        yield return new WaitForSeconds(2.5f);
        enemyAI.SetActive(true);

    }

        private IEnumerator ActivateEnemyAI2Delayed()
    {
        yield return new WaitForSeconds(3.5f); // Waits 3.5 seconds
        enemyAI2.SetActive(true);
    }

    private IEnumerator ActivateEnemyAI3Delayed()
    {
        yield return new WaitForSeconds(3.5f);
        enemyAI3.SetActive(true);
    }

    private IEnumerator ActivateEnemyAI4Delayed()
    {
        yield return new WaitForSeconds(3.5f);
        enemyAI4.SetActive(true);
    }

    private IEnumerator ActivateEnemyAI5Delayed()
    {
        yield return new WaitForSeconds(3.5f);
        enemyAI5.SetActive(true);
    }

    public void AdjustSpeed(float speedChange, float duration)
    {
        StartCoroutine(AdjustSpeedCoroutine(speedChange, duration));
    }

    private IEnumerator AdjustSpeedCoroutine(float speedChange, float duration)
    {
        speed += speedChange;
        yield return new WaitForSeconds(duration);
        speed -= speedChange;
    }


}
