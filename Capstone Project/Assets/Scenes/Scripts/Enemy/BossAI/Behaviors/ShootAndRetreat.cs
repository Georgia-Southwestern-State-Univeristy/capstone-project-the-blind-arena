using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAndRetreat : MonoBehaviour
{
    public float speed;
    public Transform target;
    public float minimumDistance;


    public GameObject projectile;
    public float timeBetweenShots;
    private float nextShotTime;

    internal void StartShootAndRetreat()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (Time.time > nextShotTime)
        {
            Instantiate(projectile, transform.position, Quaternion.identity);
            nextShotTime = Time.time + timeBetweenShots;
        }


        if (Vector3.Distance(transform.position, target.position) < minimumDistance)

        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
        }
        else
        {
            //Attack code
        }
    }


}
