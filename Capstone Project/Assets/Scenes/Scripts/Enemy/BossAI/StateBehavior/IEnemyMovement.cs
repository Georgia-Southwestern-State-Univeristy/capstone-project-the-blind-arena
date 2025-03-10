using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMovement
{
    Rigidbody RB { get; set; }

    bool IsFacingRight { get; set; }

    void MoveEnemy(Vector3 velocity);

    void CheckForLeftOrRightFacing(Vector2 velocity);
}
