using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowCaster2DProximityChecker : MonoBehaviour
{
    [SerializeField]
    private LayerMask lightLayer;
    [SerializeField]
    private ShadowCaster2D caster;

    private List<Light2D> viableLights = new();

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Light2DHelper helper = collider.gameObject.GetComponent<Light2DHelper>();
        if (helper != null)
        {
            bool isBehindLight = false;
            (Cardinal, Cardinal) helperIsAt = MazeUtils.GetCardinalDirections(transform.position, helper.transform.position);

            switch (helper.Torch.AlignedWith)
            {
                case Cardinal.North:
                    isBehindLight = helperIsAt.Item2 == Cardinal.South;
                    return;
                case Cardinal.South:
                    isBehindLight = helperIsAt.Item2 == Cardinal.North;
                    return;
                case Cardinal.East:
                    isBehindLight = helperIsAt.Item2 == Cardinal.West;
                    return;
                case Cardinal.West:
                    isBehindLight = helperIsAt.Item2 == Cardinal.East;
                    return;
            }

            if (helper.Light.shadowsEnabled && !isBehindLight)
            {
                viableLights.Add(helper.Light);
                UpdateLights();
            }
            return;
        }

        Light2D light = collider.gameObject.GetComponent<Light2D>();
        if (light.shadowsEnabled)
        {
            viableLights.Add(light);
            UpdateLights();
            return;
        }

    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        Light2DHelper helper = collider.gameObject.GetComponent<Light2DHelper>();
        if (helper == null ||
        !helper.Light.shadowsEnabled ||
        !viableLights.Contains(helper.Light)) return;

        viableLights.Remove(helper.Light);
        UpdateLights();
    }

    private void UpdateLights()
    {
        caster.enabled = viableLights.Count > 0;
    }
}
