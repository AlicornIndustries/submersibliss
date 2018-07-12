using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveForce;
    public float turnTorque;

    public TorpedoTube[] torpedoTubes;
    private int numTubes;
    private int tubeIndex = 0;
    // public type of torpedo equipped?

    public GameObject[] torpedoPrefabs;
    public int equippedTorpedoIndex = 0;

    public SonicEmitter sonicEmitter;

    private Rigidbody rb;
    private Collider coll;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        sonicEmitter = GetComponent<SonicEmitter>();
        numTubes = torpedoTubes.Length;
        foreach (TorpedoTube tube in torpedoTubes)
        {
            tube.lastFireTime = Time.time;
        }
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // TRANSLATION
        Vector3 thrust = transform.forward * moveVertical;
        rb.AddForce(thrust*moveForce);

        // ROTATION
        rb.AddRelativeTorque(Vector3.up * moveHorizontal * turnTorque);

        // TORPEDOES
        ControlFire();

        // SONAR
        if(Input.GetKeyDown(KeyCode.Space))
        {
            sonicEmitter.EmitPing();
        }
    }

    // Launch torpedoes
    private void ControlFire()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            // Has tube reloaded? (enough time past since last fire)
            if(Time.time - torpedoTubes[tubeIndex].lastFireTime >= 2f) {   // TODO
                torpedoTubes[tubeIndex].lastFireTime = Time.time;
                GameObject torpedo = (GameObject)Instantiate(torpedoPrefabs[0], torpedoTubes[tubeIndex].transform.position, Quaternion.LookRotation(torpedoTubes[tubeIndex].transform.forward));
                Physics.IgnoreCollision(coll, torpedo.GetComponent<Collider>());
                tubeIndex += 1; // go to the next tube
                if (tubeIndex >= numTubes)
                {
                    tubeIndex = 0;
                }
            }
        }
    }
}
