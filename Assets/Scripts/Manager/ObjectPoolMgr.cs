using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class PoolObjectType
{
    public string Type;

    [Min(0)] public int Count;
    [Min(0)] public int Index;

    public Component    Component;
    public GameObject   Object;
    public Transform    Container;

    public int NextIndex()
    {
        Index++;
        if (Index >= Count) Index = 0;
        
        return Index;
    }
}

[System.Serializable]
public class PoolObject
{
    public Component Component;
    public GameObject Object;

    public void Init()
    {
        Object.SetActive(false);
        Object.transform.position = Vector3.zero;
    }

    public void Spawn(Vector3 position, Vector3 eulerAngles)
    {
        Object.transform.position = position;
        Object.transform.eulerAngles = eulerAngles;
    }
}


public class ObjectPoolMgr : Singleton<ObjectPoolMgr>
{
    Transform contanerRoot;

    [SerializeField]
    List<PoolObjectType> initPoolObject = new();

    Dictionary<string, PoolObjectType> poolIndex = new();
    Dictionary<string, List<PoolObject>> poolObjectList = new();

    void Start()
    {
        contanerRoot = transform;

        initPoolObject.ForEach(e =>
        {
            e.Index = 0;
            poolObjectList[e.Type] = new();

            for (int i = 0; i < e.Count; ++i)
            {
                PoolObject pool = new PoolObject();

                pool.Object = Instantiate(e.Object);
                pool.Object.transform.SetParent(e.Container ?? contanerRoot, false);

                pool.Init();

                poolIndex[e.Type] = e;
                poolObjectList[e.Type].Add(pool);
            }
        });
    }
    

    public void AddPoolObject(string type, GameObject poolObject, int poolCount = 0, Component component = null)
    {
        if (poolIndex.ContainsKey(type))
        {
            return;
        }

        PoolObjectType poolType = new();

        poolType.Object = poolObject;
        poolType.Component = component;
        poolType.Index = 0;
        poolType.Count = poolCount;

        poolIndex[type] = poolType;
        poolObjectList[type] = new();

        for (int i = 0; i < poolCount; ++i)
        {
            PoolObject pool = new PoolObject();

            pool.Object = Instantiate(poolType.Object);
            pool.Object.transform.SetParent(contanerRoot, false);

            if (component != null)
            {
                pool.Component = pool.Object.GetComponent(component.GetType());
            }

            pool.Init();

            poolObjectList[type].Add(pool);
        }
    }

    public GameObject GetObject(string type)     { return poolObjectList[type][poolIndex[type].NextIndex()].Object; }
    public Component GetObjectComponent(string type) { return poolObjectList[type][poolIndex[type].NextIndex()].Component; }

    public PoolObject GetPoolObject(string type) { return poolObjectList[type][poolIndex[type].NextIndex()]; }

    public GameObject SpawnObject(string type, Vector3 spawnPosition, Vector3 spawnAngle)
    {
        var spawnObject = GetPoolObject(type);
        spawnObject.Spawn(spawnPosition, spawnAngle);

        return spawnObject.Object;
    }
}
