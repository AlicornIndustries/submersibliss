using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTurret : MonoBehaviour {

    public Transform barrel;
    private Rigidbody rb;
    public float turnSpeed;

    private float acceptableBarrelDeviation = 0.1f;

    private Quaternion goalRotation;

    private void Start()
    {
        rb = (Rigidbody)GetComponent(typeof(Rigidbody));
    }

    // TODO: Should we use coroutines here?
    public void ShootTarget(Vector3 location)
    {
        // Do we need to turn?
        if(!IsFacing(location))
        {
            TurnToFace(location);
        }
        else
        {
            Fire();
        }
        
    }

    // TODO: Can we optimize this?
    private bool IsFacing(Vector3 location)
    {
        float dot = Vector3.Dot(transform.forward, (location - transform.position).normalized);
        if (dot > acceptableBarrelDeviation)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Eventually, break this into turning the turret and declining the barrel
    // TODO: This doesn't work.
    private void TurnToFace(Vector3 location)
    {
        goalRotation = Quaternion.LookRotation(location - transform.position);
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, goalRotation, turnSpeed));
    }

    private void Fire()
    {
        Debug.Log("Fired turret!");
    }
}
