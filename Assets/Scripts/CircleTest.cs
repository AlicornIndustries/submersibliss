using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CircleTest : MonoBehaviour {

    public Transform orbitTarget;
    public float startOrbitDistance;
    private float startOrbitDistanceSqr;
    public float keepOrbitDistance;
    private float keepOrbitDistanceSqr;
    public float minOrbitDistance;
    private float minOrbitDistanceSqr;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = (NavMeshAgent)GetComponent(typeof(NavMeshAgent));
        startOrbitDistanceSqr = startOrbitDistance * startOrbitDistance;
        keepOrbitDistanceSqr = keepOrbitDistance * keepOrbitDistance;
        minOrbitDistanceSqr = minOrbitDistance * minOrbitDistance;
    }

    private void Update()
    {
        Vector3 offset = orbitTarget.position - transform.position;
        Debug.DrawLine(transform.position, transform.position + offset);
        float offsetSqr = offset.sqrMagnitude;

        // Too far, move closer
        if(offsetSqr > keepOrbitDistanceSqr)
        {
            agent.SetDestination(orbitTarget.position);
        }
        else if(offsetSqr < minOrbitDistanceSqr)
        {
            // Move away
        }
        else
        {
            // Circle
            Vector3 rotatedVector = Quaternion.Euler(0, 15f, 0) * -offset; // TODO: optimize this
            Debug.DrawLine(orbitTarget.position, orbitTarget.position + rotatedVector);
            agent.SetDestination(orbitTarget.position + rotatedVector);
        }
    }
}
