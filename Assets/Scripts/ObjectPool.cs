using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Instantiate objects into a dictionary in the beginning of the game.

public class ObjectPool : Singleton<ObjectPool>
{
    [System.Serializable]
    public class Pool
    {
        public PoolEnums poolEnum;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private readonly Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    private readonly Dictionary<int, Transform> poolParents = new Dictionary<int, Transform>();


    private void OnEnable()
    {
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            
            // Create child gameobjects so pooled objects will be more organized in the hierarchy
            var poolParent = new GameObject($"{pool.poolEnum} Parent");
            poolParent.transform.parent = gameObject.transform;
            poolParents[(int)pool.poolEnum] = poolParent.transform;
            
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, poolParent.transform, true);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add((int)pool.poolEnum, objectPool);
        }
    }

    public GameObject SpawnFromPool(PoolEnums poolEnum, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!poolDictionary.ContainsKey((int)poolEnum))
        {
            Debug.LogError("Pool with tag " + poolEnum + " does not exist");
            return null;
        }

        var objToSpawn = poolDictionary[(int)poolEnum].Peek();

        if (objToSpawn.activeInHierarchy)
        {
            objToSpawn = Instantiate(GetPool(poolEnum).prefab, poolParents[(int)poolEnum], true);
        }
        else
        {
            objToSpawn = poolDictionary[(int)poolEnum].Dequeue();
        }

        objToSpawn.SetActive(true);

        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        objToSpawn.transform.SetParent(parent);

        var pooledObj = objToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[(int)poolEnum].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public GameObject SpawnFromPool(PoolEnums poolEnum)
    {
        if (!poolDictionary.ContainsKey((int)poolEnum))
        {
            Debug.LogError("Pool with tag " + poolEnum + " does not exist");
            return null;
        }

        var objToSpawn = poolDictionary[(int)poolEnum].Peek();

        if (objToSpawn.activeInHierarchy)
        {
            objToSpawn = Instantiate(GetPool(poolEnum).prefab, poolParents[(int)poolEnum], true);
        }
        else
        {
            objToSpawn = poolDictionary[(int)poolEnum].Dequeue();
        }

        objToSpawn.SetActive(true);

        var pooledObj = objToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[(int)poolEnum].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public void SetEntirePool(PoolEnums poolEnum, bool value)
    {
        if (!poolDictionary.ContainsKey((int)poolEnum))
        {
            Debug.LogError("Pool with tag" + poolEnum + "does not exist");
            return;
        }
        else
        {
            foreach (Pool pool in pools)
            {
                if (pool.poolEnum != poolEnum)
                {
                    continue;
                }

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject spawnedObj = poolDictionary[(int)poolEnum].Dequeue();
                    spawnedObj.SetActive(value);
                    poolDictionary[(int)poolEnum].Enqueue(spawnedObj);
                }
            }
        }
    }

    private Pool GetPool(PoolEnums poolEnum)
    {
        return pools.Find(p => p.poolEnum.Equals(poolEnum));
    }
    
    public Transform GetPoolParent(PoolEnums poolEnum)
    {
        return poolParents[(int) poolEnum];
    }
}