using UnityEngine;

public enum PingSource { Player, PlayerTool, Enemy, Ambient };

public class SonicPing : MonoBehaviour
{
    public PingSource pingSource;

    public float expandTime = 1.2f;
    public float lifespan = 1.5f;
    //public float innerRadius = 0; // outerRadius determined by the sphere collider. IGNORED FOR NOW
    public SphereCollider outerSphere; // component of SonicPing object
    public GameObject gfx;

    private SonicDetectable sonicDetectable; // will be the thing we collide with
    //private float distance;

    private readonly float speed = 30f; // speed of sound, speed of expanding ping

    private void Update()
    {
        lifespan -= Time.deltaTime;
        if (expandTime >= 0)
        {
            expandTime -= Time.deltaTime;
            transform.localScale += new Vector3(speed*Time.deltaTime, 0, speed*Time.deltaTime);
        }
        if (lifespan <= 0)
        {
            // We want this to linger a bit after reaching end of life. Make it a max scale to reach, then have it "hang" for a bit
            // while the inner edge (purely a GFX thing) advances?
            // Or... just call it "lifespanHang" and that's when it stops expanding
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
            sonicDetectable.OnPing(pingSource, transform.position);
        }
    }
}
