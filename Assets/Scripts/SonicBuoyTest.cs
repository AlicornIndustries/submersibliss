using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicBuoyTest : MonoBehaviour {
    private SonicEmitter sonicEmitter;

    private void Start()
    {
        sonicEmitter = (SonicEmitter)GetComponent(typeof(SonicEmitter));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            sonicEmitter.EmitPing();
        }
    }
}
