using UnityEngine;

public class MaterialManager
{
    private MaterialDataBase materialDataBase;

    public MaterialManager(MaterialDataBase materialDataBase)
    {
        this.materialDataBase = materialDataBase;
    }

    public Material GetMaterial(MaterialType materialType)
    {
        Material material = materialDataBase.GetMaterial(materialType);
        return material;
    }
}
