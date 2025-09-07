using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectVisualDataBase", menuName = "Scriptable Objects/Data Bases/Status Effect Visual Database")]

public class StatusEffectVisualDataBase : ScriptableObject
{
    [SerializeField]
    private List<StatusEffectVisualInfo> materialDataList;

    private Dictionary<StatusKey, StatusEffectVisualInfo> MaterialData = new Dictionary<StatusKey, StatusEffectVisualInfo>();

    public void SetUp()
    {
        foreach (StatusEffectVisualInfo info in materialDataList)
        {
            MaterialData.Add(info.Type, info);
        }
    }

    public StatusEffectVisualInfo GetVisualInfo(StatusKey key, bool isPositive)
    {
        StatusEffectVisualInfo material = MaterialData[key];
        return material;
    }
}

[Serializable]
public struct StatusEffectVisualInfo
{
    [SerializeField]
    private Material positiveMaterial;
    [SerializeField]
    private Material negativeMaterial;
    [SerializeField]
    private StatusKey statusKey;

    public Material PositiveMaterial => positiveMaterial;
    public Material NegativeMaterial => negativeMaterial;
    public StatusKey Type => statusKey;

}

