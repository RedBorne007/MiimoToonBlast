using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    GameObject PoolObject { get; }

    string GetID();
    void Spawn();
    void Despawn();
}

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, List<IPoolable>> _poolObjDict = new Dictionary<string, List<IPoolable>>();

    public IPoolable GetOrCreate<T>(T poolObjRef) where T : MonoBehaviour, IPoolable
    {
        if (!poolObjRef)
        {
            return null;
        }

        T poolObj;

        string id = poolObjRef.GetID();

        if (!_poolObjDict.TryGetValue(poolObjRef.GetID(), out var poolObjList))
        {
            // Create new pool and new object.
            _poolObjDict.Add(id, new List<IPoolable>());

            poolObj = Instantiate(poolObjRef);
            poolObj.Spawn();

            return poolObj;
        }

        if (poolObjList.Count > 0)
        {
            // Fetch first object from pool.
            poolObj = poolObjList[0] as T;
            poolObjList.Remove(poolObj);

            poolObj.Spawn();

            return poolObj;
        }

        // Create fresh new object.
        poolObj = Instantiate(poolObjRef);
        poolObj.Spawn();

        return poolObj;
    }

    public void Despawn<T>(T poolObj) where T : MonoBehaviour, IPoolable
    {
        if (!poolObj)
        {
            return;
        }

        string id = poolObj.GetID();

        if (_poolObjDict.TryGetValue(id, out var poolObjList))
        {
            poolObjList.Add(poolObj);
            poolObj.Despawn();
        }
    }

    public void Clear()
    {
        foreach (var poolObjList in _poolObjDict.Values)
        {
            int count = poolObjList.Count;

            for (int i = 0; i < count; i++)
            {
                var poolObj = poolObjList[i];

                if (poolObj != null && poolObj.PoolObject != null)
                {
                    Destroy(poolObj.PoolObject);
                }
            }
        }

        _poolObjDict.Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}