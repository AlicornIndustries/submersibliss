using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    // Inherit from Destructable?

    enum KnowledgeLevel { Unaware, KnowsPresence, KnowsLastLocation, KnowsExactLocation };
    private KnowledgeLevel knowledgeLevel;
    private Vector3 playerLastLocation; // last location the player was seen at
    private Vector3 selfLastLocation; // last location the player saw this ship at

    public List<Transform> waypoints = new List<Transform>();
    private int waypointIndex = 0;
    private float reachProximitySqr = 2f;
    private float angleThreshold = 0.99f; // How close to exactly facing does the ship need to be before it proceeds with AdvanceTowards?

    public ShipTurret[] turrets;

    public float patrolSpeed; // these will override NavMeshAgent settings
    public float aggroSpeed;
    public float angularSpeed;
    //public float moveForce;
    //public float turnTorque;

    public float startOrbitDistance;
    private float startOrbitDistanceSqr;
    public float keepOrbitDistance;
    private float keepOrbitDistanceSqr;
    public float minOrbitDistance;
    private float minOrbitDistanceSqr;

    public int maxHealth;
    private int currentHealth;

    private SonicDetectable sonicDetectable;
    private NavMeshAgent agent;



	private void Start () {
        sonicDetectable = (SonicDetectable)GetComponent(typeof(SonicDetectable));
        agent = (NavMeshAgent)GetComponent(typeof(NavMeshAgent));
        currentHealth = maxHealth;
        knowledgeLevel = KnowledgeLevel.Unaware;

        agent.speed = patrolSpeed;
        agent.angularSpeed = angularSpeed;

        keepOrbitDistanceSqr = keepOrbitDistance * keepOrbitDistance;

	}

    private void Update()
    {
        if(knowledgeLevel == KnowledgeLevel.Unaware)
        {
            // Patrol
            //AdvanceTowards(waypoints[waypointIndex].position, patrolSpeed);
            SetDestination(waypoints[waypointIndex].position, patrolSpeed);
        }
        else if(knowledgeLevel == KnowledgeLevel.KnowsPresence)
        {
            // Still patrol?
            //AdvanceTowards(waypoints[waypointIndex].position, patrolSpeed);
            SetDestination(waypoints[waypointIndex].position, patrolSpeed);
        }
        else if(knowledgeLevel == KnowledgeLevel.KnowsLastLocation)
        {
            OrbitLocation(playerLastLocation);
            Bombard(playerLastLocation);
        }
        else if(knowledgeLevel == KnowledgeLevel.KnowsExactLocation)
        {
            // Only if player is within a static ping?
            OrbitLocation(playerLastLocation);
            // Bombard area with high spread. Call Bombard(location, spread)
            Bombard(playerLastLocation);
        }

        // Check to see if we reached a waypoint
        if (knowledgeLevel <= KnowledgeLevel.KnowsPresence) // TODO: Make this an isPatrolling check?
        {
            Vector3 offset = waypoints[waypointIndex].position - transform.position;
            float sqrLen = offset.sqrMagnitude;
            if(sqrLen < reachProximitySqr)
            {
                // Reached the waypoint
                waypointIndex += 1;
                if (waypointIndex >= waypoints.Count)
                {
                    waypointIndex = 0;
                }   
            }
        }
        
    }

    #region Damage

    public void TakeHit(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    #endregion

    #region Sonics

    public void OnPing(PingSource pingSource, Vector3 pingOrigin)
    {
        if(pingSource == PingSource.Player)
        {
            // Detect player
            knowledgeLevel = KnowledgeLevel.KnowsLastLocation;
            playerLastLocation = pingOrigin;
        }
    }

    #endregion

    #region Navigation

    private void SetDestination(Vector3 destination, float speed)
    {
        agent.speed = speed;
        agent.SetDestination(destination);
    }

    private void OrbitLocation(Vector3 location)
    {
        Vector3 offset = location - transform.position;
        Debug.DrawLine(transform.position, transform.position + offset);
        float offsetSqr = offset.sqrMagnitude;

        // Too far, move closer
        if (offsetSqr > keepOrbitDistanceSqr)
        {
            agent.SetDestination(location);
        }
        else if (offsetSqr < minOrbitDistanceSqr)
        {
            // Move away TODO
        }
        else
        {
            // Circle
            Vector3 rotatedVector = Quaternion.Euler(0, 15f, 0) * -offset; // TODO: optimize this
            Debug.DrawLine(location, location + rotatedVector);
            agent.SetDestination(location + rotatedVector);
        }

    }

    #endregion

    #region Shooting

    private void Bombard(Vector3 location)
    {
        // Call ShootTarget on each turret
        foreach(ShipTurret turret in turrets)
        {
            // TODO: add random spread as second arg
            // TODO: Only call ShootTarget on turrets that aren't already shooting
            if(!turret.hasFireOrder)
            {
                turret.FireOrder(location);
                
            }
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.DrawWireSphere(playerLastLocation, 0.8f);
    }

    #endregion


}
