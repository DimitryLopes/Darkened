using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLayerService : MonoBehaviour
{
    [SerializeField]
    private SortingLayer defaultLayer;
    [SerializeField]
    private List<SortingLayer> layers;

    public Dictionary<SortingLayer, bool> IsLayerBeingUsed = new();

    private void Start()
    {
        foreach(SortingLayer layer in layers)
        {
            IsLayerBeingUsed.Add(layer, false);
        }
    }

    public SortingLayer GetAvailableLayer()
    {
        foreach(SortingLayer layer in layers)
        {
            if (IsLayerBeingUsed[layer]) continue;
            IsLayerBeingUsed[layer] = true;
            return layer;
        }
        return defaultLayer;
    }
}
