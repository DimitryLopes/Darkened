using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : Activateable
{
    private readonly Queue<T> pool;
    private readonly T prefab;
    private readonly Transform parent;

    public Pool(T prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        pool = new Queue<T>();
    }

    public T Get()
    {
        if (pool.Count == 0)
        {
            AddObjects(1);
        }

        T obj = pool.Dequeue();
        obj.Activate();
        return obj;
    }

    public void ReturnToPool(T obj)
    {
        obj.Deactivate();
        pool.Enqueue(obj);
    }

    private void AddObjects(int count)
    {
        for (int i = 0; i < count; i++)
        {
            T newObj = Object.Instantiate(prefab, parent);
            newObj.Deactivate();
            newObj.onDeactivate.AddListener(delegate { ReturnToPool(newObj); });
            pool.Enqueue(newObj);
        }
    }
}
