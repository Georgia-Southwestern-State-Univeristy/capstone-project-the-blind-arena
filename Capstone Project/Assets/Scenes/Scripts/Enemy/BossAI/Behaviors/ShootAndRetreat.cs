using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAndRetreat : MonoBehaviour
{
    public float speed;
    public Transform target;
    public EnemyController enemyController;
    public Animator animator;
    public float minimumDistance;


    public GameObject projectile;
    public float timeBetweenShots;
    private float nextShotTime;
    private const float HEIGHT = 0.6f;

    internal void StartShootAndRetreat()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Stop moving and shoot at intervals
        if (Time.time > nextShotTime)
        {
            enemyController.IsMoving = false;
            StartCoroutine(Shoot());
            nextShotTime = Time.time + timeBetweenShots;
        }

        // Safely check if the target exists and hasn't been destroyed
        if (target != null && Vector3.Distance(transform.position, target.position) < minimumDistance)
        {
            // Retreat from the target
            transform.position = Vector3.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
            Vector3 position = transform.position;
            position.y = HEIGHT;
            transform.position = position;
        }
        else
        {
            animator.SetFloat("speed", 0);
        }
    }

    private IEnumerator Shoot() 
    {
        animator.SetTrigger("Punch");
        yield return new WaitForSeconds(.43f);
        Instantiate(projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(.43f);
        enemyController.IsMoving=true;
    }


}
