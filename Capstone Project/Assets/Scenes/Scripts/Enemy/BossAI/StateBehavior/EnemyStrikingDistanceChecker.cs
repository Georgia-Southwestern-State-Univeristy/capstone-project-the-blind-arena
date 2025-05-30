using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStrikingDistanceChecker : MonoBehaviour
{
    public GameObject PlayerTarget { get; set; }
    private Enemy _enemy;

    private void Awake()
    {
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");

        _enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject == PlayerTarget)
        {
            _enemy.SetAggroStatus(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject == PlayerTarget)
        {
            _enemy.SetAggroStatus(false);
        }
    }
}
