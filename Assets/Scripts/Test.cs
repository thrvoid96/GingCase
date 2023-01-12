using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    [field: SerializeField] public MeshFilter meshFilter { get; private set; }
    [field: SerializeField] public MeshRenderer meshRenderer{ get; private set;}
    [field: SerializeField] public MeshCollider meshCollider{ get; private set;}
    
    private void Awake()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        mesh.vertices = new Vector3[] {points[0].position, points[1].position, points[2].position, points[3].position, points[4].
            position,points[5].position,points[6].position,points[7].position,points[8].position};
        mesh.uv = new Vector2[] {points[0].position, points[1].position, points[2].position, points[3].position, points[4].
            position,points[5].position,points[6].position,points[7].position,points[8].position};
        
        mesh.triangles =  new int[] {0, 1, 2, 0,2,3,2,6,7,7,3,2};
        
        mesh.SetIndices(mesh.triangles, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals();
            
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        
        mesh.triangles =  new int[] {2,6,7,7,3,2,6,10,11,11,7,6};
    }
}
