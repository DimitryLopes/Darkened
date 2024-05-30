using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingBaker : MonoBehaviour
{
    [SerializeField]
    public Camera bakingCamera;
    [SerializeField]
    public RenderTexture renderTextureA;
    [SerializeField]
    public RenderTexture renderTextureB;
    [SerializeField]
    public LayerMask torchLayer;
    [SerializeField]
    public LayerMask staticObjectLayer;

    public void Bake(int iterations)
    {
        StartCoroutine(BakeLighting(iterations));
    }

    private IEnumerator BakeLighting(int iterations)
    {
        RenderTexture previousRT = RenderTexture.active;

        // Step 1: Render the static objects
        bakingCamera.cullingMask = staticObjectLayer;
        bakingCamera.targetTexture = renderTextureA;
        bakingCamera.Render();

        // Step 2: Render each torch light
        bakingCamera.cullingMask = torchLayer;
        for (int i = 0; i < iterations; i++)
        {
            RenderTexture.active = renderTextureB;
            GL.Clear(true, true, Color.black);
            bakingCamera.targetTexture = renderTextureB;
            bakingCamera.Render();

            Graphics.Blit(renderTextureB, renderTextureA);
        }

        RenderTexture.active = previousRT;
        bakingCamera.targetTexture = null;

        yield return null;
    }
}

