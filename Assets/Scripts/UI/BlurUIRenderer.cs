using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurUIRenderer : MonoBehaviour
{
    public Camera blurCamera;
    public Material blurMaterial;
    // Start is called before the first frame update
    void Start()
    {
        if (blurCamera.targetTexture != null)
        {
            blurCamera.targetTexture.Release();
        }
        blurCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, 1);
        blurMaterial.SetTexture("_RenTex", blurCamera.targetTexture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
