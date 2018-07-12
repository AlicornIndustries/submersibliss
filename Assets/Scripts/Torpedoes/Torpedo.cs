using UnityEngine;

public class Torpedo : MonoBehaviour {
    public float launchForce;
    public float life = 30f;
    public Rigidbody rb;

    public void Kill()
    {
        Destroy(gameObject);
    }
}
