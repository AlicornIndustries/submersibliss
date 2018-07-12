using UnityEngine;

public class SonicPing : MonoBehaviour
{
    public float lifespan = 1.2f;
    public float innerRadius = 0; // outerRadius determined by the sphere collider. IGNORED FOR NOW
    public SphereCollider outerSphere; // component of SonicPing object
    public GameObject gfx;

    private SonicDetectable sonicDetectable; // will be the thing we collide with
    //private float distance;

    private readonly float speed = 0.3f; // speed of sound, speed of expanding ping

    private void Update()
    {
        //outerSphere.radius += speed;
        //gfx.transform.localScale += new Vector3(2 * speed, 0, 2 * speed);
        transform.localScale += new Vector3(speed, 0, speed);
        innerRadius += speed;
        lifespan -= Time.deltaTime;
        if (lifespan <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the thing we collided with has a SonicDetectable component, and it's within innerRadius...
        sonicDetectable = other.gameObject.GetComponent<SonicDetectable>();
        if(sonicDetectable)
        {
            //distance = Vector3.Distance(other.transform.position, transform.position);
            //if(distance >= innerRadius)
            //{
            //    sonicDetectable.OnPing();
            //}
            sonicDetectable.OnPing();
        }
    }
}
