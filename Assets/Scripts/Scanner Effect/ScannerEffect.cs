using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerEffect : MonoBehaviour {
    // Derived from Shaders Case Study - No Man's Sky: Topographic Scanner

    public Transform ScannerOrigin;
    public Material EffectMaterial;
    public float ScanDistance;
    public float ScanSpeed;
    public float ScanLifespan;
    private float CurrentLifespan;

    public Camera _camera;

    private bool _scanning;

    private void OnEnable()
    {
        //_camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
    }

    private void Update()
    {
        if (_scanning)
        {
            ScanDistance += Time.deltaTime * ScanSpeed;
            CurrentLifespan -= Time.deltaTime;
            if(CurrentLifespan <= 0)
            {
                EndScan();
            }
        }

        // Start scan
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartScan();
        }
    }

    private void StartScan()
    {
        ScanDistance = 0;
        CurrentLifespan = ScanLifespan;
        _scanning = true;
    }

    private void EndScan()
    {
        ScanDistance = 0;
        _scanning = false;
    }



    #region Witchcraft

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        EffectMaterial.SetVector("_WorldSpaceScannerPos", ScannerOrigin.position);
        // TODO: BIG IDEA:
        // EffectMaterial.SetVector("_WorldSpaceScannerPos1",ScannerOrigin1.position);
        // EffectMaterial.Setvector("_WorldSpaceScannerPos2,ScannerOrigin2.position);
        EffectMaterial.SetFloat("_ScanDistance", ScanDistance);
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
