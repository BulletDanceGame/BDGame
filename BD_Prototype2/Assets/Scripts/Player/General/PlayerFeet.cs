using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    float _raycastDistance = 1.0f;
    public bool PlayerByLedge;

    private void FixedUpdate()
    {
        if (PlayerMovement.PlayerDirection() == new Vector2())
            return;

        CheckLedge();
        
    }

    void CheckLedge()
    {
        if (Physics2D.Raycast(transform.position, PlayerMovement.PlayerDirection(), _raycastDistance, LayerMask.GetMask("Water")))
        {
            PlayerByLedge = true;
            return;
        }
        PlayerByLedge = false;
    }

    public bool IsPlayerStandingOnGround()
    {
        if(Physics2D.Raycast(transform.position, PlayerMovement.PlayerDirection(), _raycastDistance, LayerMask.GetMask("Ground")))
            return true;
        else
            return false;
    }





    private void OnDrawGizmosSelected()
    {
        Vector3 point = transform.position + ((Vector3)PlayerMovement.PlayerDirection() * _raycastDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, point);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + -((Vector3)PlayerMovement.PlayerDirection() * 3));
    }
}
