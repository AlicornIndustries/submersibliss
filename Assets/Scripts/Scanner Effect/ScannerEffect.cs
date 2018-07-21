using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerEffect : MonoBehaviour {
    // Derived from Shaders Case Study - No Man's Sky: Topographic Scanner

    //public Transform ScannerOrigin1; // We can't use the sub for this, since the scan moves with the origin. We'll need to instantiate/object pool an empty transform each ping.
    //public Transform ScannerOrigin2;
    public static int numCircleScanners = 2;
    public Transform[] circleScannerTransforms = new Transform[numCircleScanners];
    public Vector4[] circleScannerPositions = new Vector4[numCircleScanners];
    public Material EffectMaterial;


    //public float ScanDistance;  // carthago delenda est
    public float[] circleScanDistances = new float[numCircleScanners];
    public float ScanSpeed;
    //public float ScanLifespan; // old
    //private float CurrentLifespan; // old
    public float[] circleScanLifespans = new float[numCircleScanners];
    public float[] circleScanCurrentLifespans = new float[numCircleScanners];
    public float[] circleScannings = new float[numCircleScanners]; // 0 if false, 1 if true. Shaders can't use bools or ints

    public Camera _camera;

    private void OnEnable()
    {
        _camera.depthTextureMode = DepthTextureMode.Depth;
    }

    private void Update()
    {
        // Handle currently active scanners
        for(int i = 0; i < numCircleScanners; i++)
        {
            if(circleScannings[i] > 0)
            {
                circleScanDistances[i] += Time.deltaTime * ScanSpeed;
                //ScanDistance += Time.deltaTime * ScanSpeed; // TODO
                circleScanCurrentLifespans[i] -= Time.deltaTime;
                if(circleScanCurrentLifespans[i] <= 0)
                {
                    EndScan(i);
                }
            }
        }

        // Move this into the objects that actually emit the scans, e.g. player sub calls ScannerEffect.StartScan(0) on hit Space, sonar buoys call ScannerEffect.StartScan(whatever)
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartScan(0);
        //}

        //if(Input.GetKeyDown(KeyCode.Z))
        //{
        //    StartScan(1);
        //}
    }

    public void StartScan(int scannerIndex)
    {
        circleScanDistances[scannerIndex] = 0;
        circleScanCurrentLifespans[scannerIndex] = circleScanLifespans[scannerIndex];
        circleScannings[scannerIndex] = 1;
    }

    private void EndScan(int scannerIndex)
    {
        circleScanDistances[scannerIndex] = 0;
        //ScanDistance = 0;
        circleScannings[scannerIndex] = 0;
    }

    #region Witchcraft

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        EffectMaterial.SetInt("_numCircleScanners", numCircleScanners);
        // Need to convert Vector3 positions into Vector4 so we can assign the array to the shader
        for (int i = 0; i < numCircleScanners; i++)
        {
            circleScannerPositions[i] = circleScannerTransforms[i].position; // implicitly converts to Vector4
        }
        EffectMaterial.SetVectorArray("_circleScannersWorldSpacePositions", circleScannerPositions);
        EffectMaterial.SetFloatArray("_circleScannings", circleScannings);
        EffectMaterial.SetFloatArray("_circleScanDistances", circleScanDistances);
        //EffectMaterial.SetFloat("_ScanDistance", ScanDistance);
        RaycastCornerBlit(src, dst, EffectMaterial);
    }

    void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
    {
        // Compute Frustum Corners
        float camFar = _camera.farClipPlane;
        float camFov = _camera.fieldOfView;
        float camAspect = _camera.aspect;

        float fovWHalf = camFov * 0.5f;

        Vector3 toRight = _camera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = _camera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (_camera.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (_camera.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (_camera.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (_camera.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        // Custom Blit, encoding Frustum Corners as additional Texture Coordinates
        RenderTexture.active = dest;

        mat.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }

    #endregion

}
