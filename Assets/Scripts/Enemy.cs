using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    // Inherit from Destructable?

    enum KnowledgeLevel { Unaware, KnowsPresence, KnowsLastLocation, KnowsExactLocation };
    private KnowledgeLevel knowledgeLevel;
    private Vector3 playerLastLocation; // last location the player was seen at
    private Vector3 selfLastLocation; // last location the player saw this ship at

    public List<Transform> waypoints = new List<Transform>();
    private int waypointIndex = 0;
    private float reachProximitySqr = 0.1f;
    private float angleThreshold = 0.99f; // How close to exactly facing does the ship need to be before it proceeds with AdvanceTowards?

    public float patrolSpeed;
    public float aggroSpeed;
    public float turnSpeed;
    //public float moveForce;
    //public float turnTorque;

    public int maxHealth;
    private int currentHealth;

    private SonicDetectable sonicDetectable;



	private void Start () {
        sonicDetectable = (SonicDetectable)GetComponent(typeof(SonicDetectable));
        currentHealth = maxHealth;
        knowledgeLevel = KnowledgeLevel.Unaware;
	}

    private void Update()
    {
        if(knowledgeLevel == KnowledgeLevel.Unaware)
        {
            // Patrol
            AdvanceTowards(waypoints[waypointIndex].position, patrolSpeed);
        }
        else if(knowledgeLevel == KnowledgeLevel.KnowsPresence)
        {
            // Still patrol?
            AdvanceTowards(waypoints[waypointIndex].position, patrolSpeed);
        }
        else if(knowledgeLevel == KnowledgeLevel.KnowsLastLocation)
        {
            AdvanceTowards(playerLastLocation, patrolSpeed);
        }
        else if(knowledgeLevel == KnowledgeLevel.KnowsExactLocation)
        {
            //
        }

        // Check to see if we reached a waypoint
        if (knowledgeLevel <= KnowledgeLevel.KnowsExactLocation) // TODO: Make this an isPatrolling check?
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

    #region AI

    public void OnPing(PingSource pingSource, Vector3 pingOrigin)
    {
        Debug.Log("Pinged!");
        if(pingSource == PingSource.Player)
        {
            // Detect player
            Debug.Log("Player detected!");
            knowledgeLevel = KnowledgeLevel.KnowsLastLocation; // TODO: make this just KnowsPresence, and make it patrol in a circle to try and find target
            playerLastLocation = pingOrigin;
        }
    }

    #endregion

    #region Navigation

    // TODO: Make these use rigidbody instead

    private void AdvanceTowards(Vector3 location, float speed)
    {
        
        // First turn to face
        if(!IsFacing(location))
        {
            Debug.Log("is not facing");
            Vector3 targetDirection = location - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        // If we're already facing
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, location, speed * Time.deltaTime);
        }
    }

    private bool IsFacing(Vector3 location)
    {
        float dot = Vector3.Dot(transform.forward, (location - transform.position).normalized);
        if(dot>angleThreshold)
        {
            return true;
        }
        else
        {
            return false;
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
