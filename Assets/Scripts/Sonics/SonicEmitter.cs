using UnityEngine;

public class SonicEmitter : MonoBehaviour {

    public SonicPing sonicPingPrefab;
    public PingSource pingSource;

    private Vector3 origin;

    // Called by holder object, e.g. when moving, call this.
    // Perhaps have multiple ping prefabs for movement, shooting, etc.
    public void EmitPing()
    {
        SonicPing ping = Instantiate(sonicPingPrefab, transform);
        ping.pingSource = pingSource;
    }
}
