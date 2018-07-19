using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTurret : MonoBehaviour {

    public Transform projectileEmitter;
    private Rigidbody rb;
    public float turnSpeed;

    public bool hasFireOrder;
    public float refireDelay;
    public float timeLastFired;

    //private float acceptableBarrelDeviation = 0.1f;
    private float facingThreshold = 0.999f; // IsFacing uses dot, which goes to 1 as it gets closer to facing exactly
    private Quaternion goalRotation;

    private void Start()
    {
        rb = (Rigidbody)GetComponent(typeof(Rigidbody));
        hasFireOrder = false;
        //timeLastFired = Time.time - refireDelay; // so we can fire right out of the gate
        timeLastFired = Time.time;
    }

    public void FireOrder(Vector3 location)
    {
        hasFireOrder = true;
        StartCoroutine(ShootTarget(location));
    }

    IEnumerator ShootTarget(Vector3 location)
    {
        // Turn to face, then shoot once, then break

        while (!IsFacing(location))
        {
            // Turn to face a little bit
            TurnToFace(location);
            yield return null;
        }
        //Facing, now find out if we are loaded:
        while (!CanFire())
        {
            yield return null;
        }
        Fire();
        hasFireOrder = false;
        yield break;
    }

    #region Aiming

    //TODO: Can we optimize this?
    private bool IsFacing(Vector3 location)
    {
        float dot = Vector3.Dot(transform.forward, (location - transform.position).normalized);
        if (dot > facingThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Eventually, break this into turning the turret and declining the barrel
    private void TurnToFace(Vector3 location)
    {
        goalRotation = Quaternion.LookRotation(location - transform.position);
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, goalRotation, turnSpeed));
        //rb.MoveRotation(goalRotation);
    }

    #endregion

    #region Shooting

    private void Fire()
    {
        Debug.Log("Fired turret!");
        timeLastFired = Time.time;
    }

    private bool CanFire()
    {
        //Debug.Log("Time: " + Time.time.ToString() + "\nrefireDelay + timeLastFired: " + (refireDelay+timeLastFired).ToString());
        if (Time.time > (refireDelay + timeLastFired))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
