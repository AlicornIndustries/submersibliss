using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicDetectable : MonoBehaviour {

    public bool isEnemy;
    private Enemy enemy;

    private bool audible = false; // is the object currently "visible" to the player's sonar?
    private float inaudibleDelay = 3f; // seconds after being pinged before it becomes inaudible again

    public MeshRenderer meshRenderer;
  
    private void Start()
    {
        meshRenderer.enabled = false;
        if(isEnemy)
        {
            enemy = (Enemy)GetComponent(typeof(Enemy));
        }
    }

    // Called when collides with a SonicPing sphere collider
    public void OnPing(PingSource pingSource, Vector3 pingOrigin) // see also https://stackoverflow.com/questions/2844899/how-to-get-global-access-to-enum-types-in-c for possible improvements
    {
        Debug.Log("pinged!");
        BecomeAudible();
        if(isEnemy)
        {
            // Tell the enemy component that we got pinged
            enemy.OnPing(pingSource, pingOrigin);
        }
    }

    private void BecomeAudible()
    {
        audible = true;
        meshRenderer.enabled = true;
        Invoke("BecomeInaudible", inaudibleDelay);
    }

    private void BecomeInaudible()
    {
        audible = false;
        meshRenderer.enabled = false;
    }
}
