using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerManager : MonoBehaviour {

    #region Singleton

    public static ScannerManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public ScannerEffect scannerEffect;

}
