using UnityEngine;

public class SonicEmitter : MonoBehaviour {

    public SonicPing sonicPingPrefab;
    public PingSource pingSource;
    public int scannerIndex;

    private Vector3 origin;

    // Called by holder object, e.g. when moving, call this.
    // Perhaps have multiple ping prefabs for movement, shooting, etc.
    public void EmitPing()
    {
        SonicPing ping = Instantiate(sonicPingPrefab, transform);
        ping.pingSource = pingSource;
        ScannerManager.instance.scannerEffect.StartScan(scannerIndex);
        // Object pool, make an empty and modify StartScan to take the transform of the empty as the origin?
    }
}
