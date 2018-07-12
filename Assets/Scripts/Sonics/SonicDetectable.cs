using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicDetectable : MonoBehaviour {

    private MeshRenderer meshRenderer;

    private bool audible = false; // is the object currently "visible" to the player's sonar?
    private float timeLastPinged = 0f;

    private void Start()
    {
        meshRenderer = (MeshRenderer)GetComponent(typeof(MeshRenderer));
        meshRenderer.enabled = false;
    }

    // Called when collides with a SonicPing sphere collider
    public void OnPing()
    {
        Debug.Log("pinged!");
        timeLastPinged = Time.time;
        BecomeAudible();
    }

    private void BecomeAudible()
    {
        audible = true;
        meshRenderer.enabled = true;
    }
}
