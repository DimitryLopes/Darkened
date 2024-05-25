using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    private readonly Dictionary<System.Type, object> pools = new Dictionary<System.Type, object>();

    private void CreatePool<T>(T prefab, Transform parent) where T : Activateable
    {
        var pool = new Pool<T>(prefab, parent);
        pools[typeof(T)] = pool;
    }

    public T Get<T>(T prefab, Transform parent) where T : Activateable
    {
        if (!pools.ContainsKey(typeof(T)))
        {
            CreatePool(prefab, parent);
        }
        T obj = (pools[typeof(T)] as Pool<T>).Get();
        return obj;
    }
}
