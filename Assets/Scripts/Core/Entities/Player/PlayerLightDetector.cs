using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using Zenject;

public class PlayerLightDetector : MonoBehaviour
{

    [Header("Detection Settings")]
    [SerializeField] private float maxCheckDistance = 10f;
    [SerializeField] private LayerMask lightBlockingLayers;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Collider2D playerCollider;

    private static readonly List<Light2D> activeLights = new();

    public bool IsLit { get; private set; }

    public void Setup(SignalBus signalBus)
    {
        signalBus.Subscribe<OnTorchLitSignal>(RegisterLight);
        signalBus.Subscribe<OnTorchExtinguishedSignal>(UnregisterLight);
    }

    void Update()
    {
        IsLit = false;
        Vector3 playerPos = playerCollider.bounds.center;

        foreach (var light in activeLights)
        {
            if (light == null) continue;

            //Distance prefilter
            float dist = Vector3.Distance(light.transform.position, playerPos);
            if (dist > maxCheckDistance || dist > light.pointLightOuterRadius)
                continue;

            //Raycast check
            Vector3 dir = (playerPos - light.transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(
                light.transform.position,
                dir,
                dist,
                lightBlockingLayers | playerLayer
            );

            if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
            {
                IsLit = true;
                break;
            }
        }
    }

    private void RegisterLight(OnTorchLitSignal signal)
    {
        var light = signal.Light;
        if (!activeLights.Contains(light))
            activeLights.Add(light);
    }

    private void UnregisterLight(OnTorchExtinguishedSignal signal)
    {
        var light = signal.Light;
        activeLights.Remove(light);
    }
}
