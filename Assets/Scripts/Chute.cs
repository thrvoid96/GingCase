using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chute : MonoBehaviour, IPooledObject
{
    //[field: SerializeField] public MeshFilter meshFilter { get; private set; }
    //[field: SerializeField] public MeshCollider meshCollider{ get; private set;}
    [field: SerializeField] public Rigidbody rb{ get; private set;}
    public void OnObjectSpawn()
    {
        
    }

    public void OnObjectDeactivated()
    {
        //meshFilter.mesh = null;
        //meshCollider.sharedMesh = null;
        transform.parent.SetParent(ObjectPool.Instance.GetPoolParent(PoolEnums.Chute));
        transform.parent.gameObject.SetActive(false);
    }
}
