
using UnityEngine;

public class IronTorpedo : Torpedo {
    // launchForce = 10f
    public float explosionRadius = 5f;
    public float explosionForce = 1f;
    public float reloadTime = 2f; // TODO: integrate this better

    private bool hasExploded = false;

    private void Start()
    {
        rb.AddRelativeForce(Vector3.forward * launchForce);
        Invoke("Kill", life); // remove itself automatically if life elapses
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!hasExploded)
        {
            hasExploded = true;
            Explode();
        }

    }

    private void Explode()
    {
        // Instantiate(explosionPrefab, transform.position, transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce,transform.position,explosionRadius);
            }
        }
        Destroy(gameObject);
    }
}
