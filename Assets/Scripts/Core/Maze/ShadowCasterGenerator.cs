using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class ShadowCasterGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform shadowContainer;
    [SerializeField]
    private ShadowCasterHelper helperPrefab;
    [SerializeField]
    private bool selfShadows = true;

    private List<ShadowCasterHelper> Helpers = new();

    static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
                                .Assembly
                                .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                                .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

    public void Create(CompositeCollider2D collider)
    {
        DeactivateAllCasters();
        for (int i = 0; i < collider.pathCount; i++)
        {
            Vector2[] pathVertices = new Vector2[collider.GetPathPointCount(i)];
            collider.GetPath(i, pathVertices);
            ShadowCaster2D shadowCaster = GetAvailableShadowCaster();
            shadowCaster.selfShadows = selfShadows;

            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++)
            {
                testPath[j] = pathVertices[j];
            }

            shapePathField.SetValue(shadowCaster, testPath);
            shapePathHashField.SetValue(shadowCaster, Random.Range(int.MinValue, int.MaxValue));
            meshField.SetValue(shadowCaster, new Mesh());
            generateShadowMeshMethod.Invoke(shadowCaster,
            new object[] { meshField.GetValue(shadowCaster), shapePathField.GetValue(shadowCaster) });
        }
    }

    private ShadowCaster2D GetAvailableShadowCaster()
    {
        foreach(var helper in Helpers)
        {
            if (helper.IsActive) continue;
            helper.Activate();
            return helper.ShadowCaster;
        }

        var newHelper = Instantiate<ShadowCasterHelper>(helperPrefab, shadowContainer);
        newHelper.Activate();
        Helpers.Add(newHelper);
        return newHelper.ShadowCaster;
    }

    private void DeactivateAllCasters()
    {
        foreach(var helper in Helpers)
        {
            helper.Deactivate();
        }
    }

}