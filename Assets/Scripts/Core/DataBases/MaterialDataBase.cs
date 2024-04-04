using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialDataBase", menuName = "Scriptable Objects/Data Bases/Material Data Base")]
public class MaterialDataBase : ScriptableObject
{
    [SerializeField]
    private List<MaterialInfo> materialDataList;

    private Dictionary<MaterialType, Material> MaterialData = new Dictionary<MaterialType, Material>();

    public void SetUp()
    {
        foreach (MaterialInfo info in materialDataList)
        {
            MaterialData.Add(info.Type, info.Material);
        }
    }

    public Material GetMaterial(MaterialType key)
    {
        Material material = MaterialData[key];
        return material;
    }
}

[Serializable]
public struct MaterialInfo
{
    [SerializeField]
    private MaterialType materialType;
    [SerializeField]
    private Material material;

    public Material Material => material;
    public MaterialType Type => materialType;

}

public enum MaterialType
{
    Default,
    Outline,
}
