using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class Light2DHelper : MonoBehaviour
{
    public MazeTorch Torch;
    public Light2D Light;
    public List<ShadowCaster2DProximityChecker> AffectedCasters = new();

    public void OnShadowCasterEnter(ShadowCaster2DProximityChecker shadowCaster)
    {
        AffectedCasters.Add(shadowCaster);

    }

    public void OnShadowCasterExit(ShadowCaster2DProximityChecker shadowCaster)
    {
        AffectedCasters.Remove(shadowCaster);

    }
}
