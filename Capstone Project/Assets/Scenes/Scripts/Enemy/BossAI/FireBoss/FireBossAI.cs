using System;
using System.Collections;
using UnityEngine;

public class FireBossAI : MonoBehaviour
{
    public float speed;
    public float dashLength = 15f;
    public float dashDuration = 1f;
    public float dashCooldown = 3f;
    public Transform target;
    public Transform returnWaypoint;
    public float minimumDistance = 5f;
    public float attackDelay = 2f;
    public float projectileAttackRate = 1.5f;
    public float retreatSpeed = 10f;
    public Animator animator;
    public GameObject[] attackPrefabs;

    private EnemyHealth enemyHealth;
    private float initialSpeed;
    private bool isDashing;
    private bool startPhaseOne=false;
    private bool startPhaseTwo=false;
    private bool startPhaseThree=false;
    private bool isFocused;
    private bool interruptMovement;
    private bool attackLock, dashLock, genLock, proLock, stopDash;
    private bool targetLock = false;
    private bool p1, p2, p3;
    private const float HEIGHT = 0.6f;
    private System.Random rnd = new System.Random();
    private int random;

    [SerializeField] private AudioSource walkingAudioSource;

    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioSource sfxAudioSource;

    public EarthBossAI earthBoss;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        Rigidbody rb = GetComponent<Rigidbody>();
        initialSpeed = speed;

        if (!enemyHealth) Debug.LogError("EnemyHealth component not found!");
    }

    private void Update()
    {
        if (!targetLock)
        {
            targetLock = true;
            StartCoroutine(CheckForTarget());
        }

        // Overheat Phase (25% health)
        if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.25f)
        {
            if (!startPhaseThree)
            {
                startPhaseThree = true;
                StopAllCoroutines();
                interruptMovement = false;
                attackLock = false;
                genLock = false;
                StartCoroutine(PhaseThree());
            }
            if (Vector3.Distance(transform.position, target.position) > minimumDistance*1.5 && !interruptMovement && ! isFocused)
            {
                MoveTowardsPlayer();
            }
            else
            {
                animator.SetFloat("speed", 0);

                if (walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.Stop();
                }
            }
            return;
        }

        // Turret Phase (66% health)
        else if (enemyHealth.currentHealth <= enemyHealth.maxHealth * 0.66f)
        {
            if (!startPhaseTwo)
            {
                startPhaseTwo = true;
                StopAllCoroutines();
                interruptMovement = false;
                attackLock = false;
                dashLock = false;
                StartCoroutine(PhaseTwo());
            }
            if (Vector3.Distance(transform.position, returnWaypoint.position) > 1f && !interruptMovement)
            {
                genLock = false;
                MoveTowardsWapoint();
            }
            else
            {
                genLock = true;
                animator.SetFloat("speed", 0);

                if (walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.Stop();
                }
            }
            return;
        }

        else
        {
            if (!startPhaseOne)
            {
                startPhaseOne = true;
                interruptMovement = false;
                attackLock = false;
                StartCoroutine(PhaseOne());
            }
            if (Vector3.Distance(transform.position, target.position) > minimumDistance && !interruptMovement)
            {
                MoveTowardsPlayer();
            }
            else
            {
                animator.SetFloat("speed", 0);

                if (walkingAudioSource.isPlaying)
                {
                    walkingAudioSource.Stop();
                }
            }
            return;
        }
    }

    private IEnumerator CheckForTarget()
    {
        System.Random rand = new System.Random();
        int newTarg = rand.Next(0,3);
        switch (newTarg)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
        target = FindFirstObjectByType<PlayerController>().transform;
        yield return new WaitForSeconds(10f);
        targetLock = false;
    }

    private void MoveTowardsPlayer()
    {
        if (isDashing)
        {
            speed = initialSpeed*1.5f;
        }
        else
        {
            speed = initialSpeed;
        }
        Vector3 direction = (target.position - transform.position).normalized;
        direction *= ((Math.Abs(direction.z) * .6f) + 1) * speed;
        animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - direction.magnitude));
        FlipSprite(direction.x);

        Vector3 position = transform.position;
        position.y = HEIGHT;
        transform.position = position;

        transform.position += direction * Time.deltaTime;

        if (!walkingAudioSource.isPlaying)
        {
            walkingAudioSource.Play();
        }
    }

    private void MoveTowardsWapoint()
    {
        Vector3 direction = (returnWaypoint.position - transform.position).normalized;
        direction *= ((Math.Abs(direction.z) * .6f) + 1) * speed;
        animator.SetFloat("speed", Mathf.Abs(transform.position.magnitude - direction.magnitude));
        FlipSprite(direction.x);

        Vector3 position = transform.position;
        position.y = HEIGHT;
        transform.position = position;

        transform.position += direction * Time.deltaTime;
    }

    private IEnumerator PhaseOne()
    {
        Debug.Log("Phase One");
        while (startPhaseOne)
        {
            if (!attackLock)
            {
                random = rnd.Next(0, 2);
                Debug.Log(random);
            }
            attackLock = true;
                switch (random)
                {
                    //Move close and Dash Attack
                    case 0:
                    for (int i = 0; i < 40; i++)
                    {
                        isDashing = true;
                        p3 = false;
                        if (Vector3.Distance(transform.position, target.position) <= minimumDistance)
                        {
                            StartCoroutine(DashAttack(1));
                        }
                        yield return new WaitForSeconds(0.25f);
                    }
                        break;
                    //Move and Projectile Attack
                    case 1:
                    for (int i = 0; i < 10; i++)
                    {
                        isDashing = false;
                        p3 = true;
                        if (!proLock)
                        {
                            StartCoroutine(ShootProjectile(0, 1, 1));
                        }
                        yield return new WaitForSeconds(1);
                    }
                        break;
            }
            attackLock = false;
        }
    }

    private IEnumerator PhaseTwo()
    {
        Debug.Log("Phase Two");
        while (startPhaseTwo)
        {
            // If we reached the waypoint, shoot patterns
            if (genLock)
            {
                Debug.Log("Reached Waypoint");
                if (!attackLock)
                {
                    attackLock = true;
                    p1 = true;
                    p2 = false; p3 = false;
                    StartCoroutine(ProjectileArc(90, 4, 1, 5));
                    yield return new WaitForSeconds(projectileAttackRate*5 + 3);
                    p2 = true;
                    p1 = false; p3 = false;
                    StartCoroutine(SlowArc(180, 3, 2, 3));
                    yield return new WaitForSeconds(projectileAttackRate*5 + 3);
                    p3 = true;
                    p1 = false; p2 = false;
                    StartCoroutine(ShootProjectile(3,5, 1));
                    yield return new WaitForSeconds(projectileAttackRate*5 + 4);
                    attackLock = false;
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator PhaseThree()
    {
        Debug.Log("Phase Three");
        while (startPhaseThree)
        {
            Debug.Log("Striking Distance");
            if (!attackLock)
            {
                attackLock = true;
                //Rushing
                isDashing = true;
                for (int i = 0; i < 5; i++)
                {
                    StartCoroutine(DashAttack(2));
                    Debug.Log("Attempt Rush");
                    yield return new WaitForSeconds(dashCooldown + dashDuration + 1f);
                }
                yield return new WaitForSeconds(1f);
                //Fire Stars
                isFocused = true;
                isDashing = false;
                p1 = true;
                p3 = true;
                for (int i = 0; i < 5; i++)
                {
                    Debug.Log("Attempt Walls");
                    StartCoroutine(ProjectileArc(360, 2, 3, 1));
                    yield return new WaitForSeconds(.1f);
                    StartCoroutine(ProjectileArc(300, 2, 3, 1));
                    yield return new WaitForSeconds(.1f);
                    StartCoroutine(ProjectileArc(240, 2, 3, 1));
                    yield return new WaitForSeconds(.1f);
                    StartCoroutine(ProjectileArc(180, 2, 3, 1));
                    yield return new WaitForSeconds(.1f);
                    StartCoroutine(ProjectileArc(158, 2, 3, 1));
                    yield return new WaitForSeconds(.1f);
                    StartCoroutine(ProjectileArc(130, 2, 3, 1));
                    yield return new WaitForSeconds(.1f);
                    StartCoroutine(ProjectileArc(103, 2, 3, 1));
                    yield return new WaitForSeconds(.1f);
                    StartCoroutine(ProjectileArc(80, 2, 3, 1));
                    yield return new WaitForSeconds(.1f);
                    Debug.Log("Attempt Stream");
                    StartCoroutine(ShootProjectile(0, 20, 10));
                    yield return new WaitForSeconds((projectileAttackRate/10+.5f)*10);
                    yield return new WaitForSeconds(3f);
                }
                //Rest
                p1 = false;
                Debug.Log("Attempt Rest");
                yield return new WaitForSeconds(20f);
                isFocused = false;
                attackLock = false;
            }
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    private IEnumerator DashAttack(float magnitude)
    {
        if (!dashLock)
        {
            animator.SetTrigger("Dash");
            Debug.Log("Start Dash");
            dashLock = true;
            interruptMovement = true;
            yield return new WaitForSeconds(0.25f);

            Vector3 dashDirection = (target.position - transform.position).normalized;
            Vector3 dir = dashDirection;
            dir.y = 0;
            dashDirection = dir;
            dashDirection *= ((Math.Abs(dashDirection.z) * .6f) + 1);
            float elapsedTime = 0f;
            FlipSprite(dashDirection.x);

            StartCoroutine(DashTrail());
            while (elapsedTime < dashDuration && !stopDash)
            {
                transform.position += dashDirection * dashLength * Time.deltaTime * magnitude;
                FlipSprite(dashDirection.x);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            StopCoroutine(DashTrail());
            yield return new WaitForSeconds(dashCooldown);
            interruptMovement = false;
            dashLock=false;
        }
    }

    private IEnumerator DashTrail()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject projectile = Instantiate(attackPrefabs[4], transform.position, Quaternion.identity);
            yield return new WaitForSeconds(dashDuration/10);
            earthBoss.PlayAttackSound(1);
        }
    }

    private IEnumerator ShootProjectile(int type, int amount, float wait)
    {
        proLock = true;
        if (p3)
        {
            for (int i = 0; i < amount; i++)
            {
                if (attackPrefabs.Length > type)
                {
                    animator.SetTrigger("Punch");
                    interruptMovement = true;
                    yield return new WaitForSeconds(0.2f);

                    earthBoss.PlayAttackSound(1);

                    GameObject projectile = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                    ProjectileAttack attack = projectile.GetComponent<ProjectileAttack>();
                    attack.target = target;
                    FlipSprite((target.position - transform.position).normalized.x);
                    yield return new WaitForSeconds(0.1f);
                    interruptMovement = false;
                }
                yield return new WaitForSeconds(projectileAttackRate /wait);
            }
            proLock = false;
        }
    }

    private IEnumerator ProjectileArc(int arc, int count, int type, int amount)
    {
        proLock=true;
        int arcAngle = arc, projectileCount = count;
        float startAngle = -arcAngle / 2f;
        float angleStep = arcAngle / (projectileCount - 1);

        if (p1)
        {
            for (int i = 0; i < amount; i++)
            {
                animator.SetTrigger("Point");
                interruptMovement = true;
                yield return new WaitForSeconds(0.2f);

                earthBoss.PlayAttackSound(0);

                Vector3 directionToTarget = (target.position - transform.position).normalized;
                FlipSprite(directionToTarget.x);
                for (int j = 0; j < projectileCount; j++)
                {
                    float angle = startAngle + (angleStep * j);
                    Vector3 spreadDirection = Quaternion.Euler(0, angle, 0) * directionToTarget;

                    GameObject projectile = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                    ProjectileAttack attack = projectile.GetComponent<ProjectileAttack>();
                    attack.Init(target.transform, spreadDirection);
                }
                yield return new WaitForSeconds(0.1f);
                interruptMovement = false;
                yield return new WaitForSeconds(projectileAttackRate);
            }
        }
        proLock=false;
    }

    private IEnumerator SlowArc(int arc, int count, int type, int amount)
    {
        proLock = true;
        int arcAngle = arc, projectileCount = count;
        float startAngle = -arcAngle / 2f;
        float angleStep = arcAngle / (projectileCount - 1);

        if (p2)
        {
            for (int i = 0; i < amount; i++)
            {
                animator.SetTrigger("Clap");
                interruptMovement = true;
                yield return new WaitForSeconds(0.2f);

                

                Vector3 directionToTarget = (target.position - transform.position).normalized;
                FlipSprite(directionToTarget.x);
                for (int j = 0; j < projectileCount; j++)
                {
                    float angle = startAngle + (angleStep * j);
                    Vector3 spreadDirection = Quaternion.Euler(0, angle, 0) * directionToTarget;

                    GameObject projectile = Instantiate(attackPrefabs[type], transform.position, Quaternion.identity);
                    ProjectileAttack attack = projectile.GetComponent<ProjectileAttack>();
                    attack.Init(target.transform, spreadDirection);
                    earthBoss.PlayAttackSound(1);
                }
                yield return new WaitForSeconds(0.1f);
                interruptMovement = false;
                yield return new WaitForSeconds(projectileAttackRate * 1.66f);
            }
        }
        proLock = false;
    }

    private void FlipSprite(float directionX)
    {
        transform.localScale = new Vector3(Mathf.Sign(directionX) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);      
        }
        if (collision.CompareTag("Wall"))
        {
            Debug.Log("Enemy In Wall");
            stopDash = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        stopDash = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("Enemy In Wall");
            stopDash = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {

        stopDash = false;
    }

    private void HandlePlayerCollision(GameObject player)
    {
        Debug.Log("Boss hit the player!");
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.Damage(20);
        }
    }
}
