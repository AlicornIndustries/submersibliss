using UnityEngine;

public class Torpedo : MonoBehaviour {
    public float launchForce;
    public float life = 5f;
    public Rigidbody rb;

    public void Kill()
    {
        Destroy(gameObject);
    }
}
