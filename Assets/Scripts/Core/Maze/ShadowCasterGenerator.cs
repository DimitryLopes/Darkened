using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

[RequireComponent(typeof(Collider2D))]
public class ShadowCasterGenerator : MonoBehaviour
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private Transform shadowContainer;
    [SerializeField]
    private ShadowCasterHelper helperPrefab;
    [SerializeField]
    private bool selfShadows = false;

    private List<ShadowCasterHelper> Helpers = new();
    private readonly PathEqualityComparer pathComparer = new(1e-3f);

    private CompositeCollider2D lastCollider;
    private List<Vector2[]> lastPaths = new();
    private Dictionary<Vector2[], ShadowCasterHelper> helperMap = new ();

    // Reflection cache
    static readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathHashField = typeof(ShadowCaster2D).GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
                                .Assembly
                                .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                                .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);
    private void OnEnable()
    {
        signalBus.Subscribe<OnMazeChangedDinamicallySignal>(OnMazeChangedDinamically);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<OnMazeChangedDinamicallySignal>(OnMazeChangedDinamically);
    }

    public void Create(CompositeCollider2D collider)
    {
        DeactivateAllCasters();
        lastCollider = collider;
        lastPaths.Clear();
        helperMap.Clear();

        for (int i = 0; i < collider.pathCount; i++)
        {
            Vector2[] pathVertices = new Vector2[collider.GetPathPointCount(i)];
            collider.GetPath(i, pathVertices);
            var path = (Vector2[])pathVertices.Clone();
            lastPaths.Add(path);
            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++)
            {
                testPath[j] = pathVertices[j];
            }

            var helper = GetAvailableHelper();
            ShadowCaster2D shadowCaster = helper.ShadowCaster;
            helperMap.Add(path, helper);

            SetupHelper(shadowCaster, testPath);
        }
    }

    private static void SetupHelper(ShadowCaster2D shadowCaster, Vector3[] testPath)
    {
        shapePathField.SetValue(shadowCaster, testPath);
        shapePathHashField.SetValue(shadowCaster, Random.Range(int.MinValue, int.MaxValue));
        meshField.SetValue(shadowCaster, new Mesh());
        generateShadowMeshMethod.Invoke(shadowCaster,
        new object[] { meshField.GetValue(shadowCaster), shapePathField.GetValue(shadowCaster) });
    }

    #region Pooling
    private ShadowCasterHelper GetAvailableHelper()
    {
        foreach(var helper in Helpers)
        {
            if (helper.IsActive) continue;
            helper.Activate();
            return helper;
        }

        var newHelper = Instantiate(helperPrefab, shadowContainer);
        newHelper.Activate();
        Helpers.Add(newHelper);
        newHelper.ShadowCaster.selfShadows = selfShadows;
        return newHelper;
    }
    #endregion
    private Vector2[] GetPathClone(CompositeCollider2D collider, int index)
    {
        int count = collider.GetPathPointCount(index);
        var verts = new Vector2[count];
        collider.GetPath(index, verts);
        return (Vector2[])verts.Clone();
    }

    private void OnMazeChangedDinamically(OnMazeChangedDinamicallySignal signal)
    {
        if (lastCollider == null) return;

        List<Vector2[]> newPaths = new();
        for (int i = 0; i < lastCollider.pathCount; i++)
        {
            newPaths.Add(GetPathClone(lastCollider, i));
        }

        var removedPaths = lastPaths.Except(newPaths, pathComparer).ToList();
        var addedPaths = newPaths.Except(lastPaths, pathComparer).ToList();

        foreach (var path in removedPaths)
        {
            if (helperMap.TryGetValue(path, out ShadowCasterHelper helper))
            {
                helper.Deactivate();
                helperMap.Remove(path);
            }
        }

        foreach (Vector2[] path in addedPaths)
        {
            UpdatePath(path);
        }

        lastPaths = newPaths;
    }

    private void UpdatePath(Vector2[] path)
    {
        ShadowCasterHelper helper = GetAvailableHelper();
        helperMap.Add(path, helper);
        Vector3[] parsedPath = new Vector3[path.Length];
        for(int i = 0; i < path.Length; i++)
        {
            parsedPath[i] = path[i];
        }
        SetupHelper(helper.ShadowCaster, parsedPath);
    }

    private void DeactivateAllCasters()
    {
        foreach(var helper in Helpers)
        {
            helper.Deactivate();
        }
    }

    private sealed class PathEqualityComparer : IEqualityComparer<Vector2[]>
    {
        private readonly float eps;
        public PathEqualityComparer(float epsilon) { eps = Mathf.Max(1e-6f, epsilon); }

        public bool Equals(Vector2[] a, Vector2[] b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (!Approximately(a[i], b[i], eps)) return false;
            }
            return true;
        }

        public int GetHashCode(Vector2[] arr)
        {
            if (arr == null) return 0;
            unchecked
            {
                int hash = 17;
                for (int i = 0; i < arr.Length; i++)
                {
                    int qx = Quantize(arr[i].x);
                    int qy = Quantize(arr[i].y);
                    hash = hash * 31 + qx;
                    hash = hash * 31 + qy;
                }
                hash = hash * 31 + arr.Length;
                return hash;
            }
        }

        private int Quantize(float v) => Mathf.RoundToInt(v / eps);
        private static bool Approximately(Vector2 a, Vector2 b, float e)
            => Mathf.Abs(a.x - b.x) <= e && Mathf.Abs(a.y - b.y) <= e;
    }
}