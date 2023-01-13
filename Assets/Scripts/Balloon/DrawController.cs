using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class DrawController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler
{
    private List<Vector3> pointsList = new List<Vector3>();
    [SerializeField] private LineRenderer lineRenderer;
    
    private bool canDraw;
    private Chute chuteObj;
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        canDraw = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        canDraw = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Add the first point to list on touch
        Vector3 touchPos = Camera.main.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 7f)));
        UpdatePointsList(touchPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Draw(eventData.position);
    }
    
    private void Draw(Vector3 position)
    {
        if (canDraw)
        {
            var startPoint = pointsList[pointsList.Count - 1];
            Vector3 touchPos = Camera.main.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, 7f)));
            
            var distance = Vector3.Distance(startPoint, touchPos);
            
            // Dont take the input if it's too close to the last one
            if (distance< 0.1f) { return; }
            
            float pointDistance = 0.1f;
            int numPoints = (int)(distance / pointDistance);
            float increment = 1.0f / (numPoints + 1);

            // Fill in the blanks if the last two inputs were too apart from each other
            for (int i = 0; i < numPoints; i++) {
                float t = increment * (i + 1);
                
                Vector3 point = Vector3.Lerp(startPoint, touchPos, t);
                UpdatePointsList(point);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Deactivate the last object
        if(chuteObj != null) 
        {
            chuteObj.TryGetComponent<IPooledObject>(out var deactivateable);
            deactivateable?.OnObjectDeactivated();
        }
        
        //OLD CODE FOR CREATING MESH FROM POINTS AT RUNTIME (You can test it if you want, but there were some problems and the code is not optimized)
        //CreateMesh();
        
        CreateFakeMesh();
        pointsList.Clear();
        lineRenderer.positionCount = 0;
    }

    private void CreateFakeMesh()
    {
        chuteObj = ObjectPool.Instance.SpawnFromPool(PoolEnums.Chute,Vector3.zero , Quaternion.identity, null).transform.GetChild(0).GetComponent<Chute>();
        
        //Spawn the chutePart object on every 3 points because of the size
        for (int i = 0; i < pointsList.Count; i+=3)
        {
            ObjectPool.Instance.SpawnFromPool(PoolEnums.ChutePart, pointsList[i], Quaternion.Euler(new Vector3(90f,0f,0f)), chuteObj.transform);
        }
        
        //Set the parent object's center to the spawned chute parts so we can move the parent to a desired position if needed
        var parent = chuteObj.transform.parent;
        var center = chuteObj.rb.centerOfMass;
        chuteObj.transform.SetParent(null);
        parent.transform.position = center;
        chuteObj.transform.SetParent(parent);
        parent.transform.position = Vector3.zero;
    }
    
    private void UpdatePointsList(Vector3 position)
    {
        pointsList.Add(position);
        lineRenderer.positionCount = pointsList.Count;
        lineRenderer.SetPositions(pointsList.ToArray());
    }
    
    // private void CreateMesh()
    // {
    //     if (pointsList.Count >= 3)
    //     {
    //         chuteObj = ObjectPool.Instance.SpawnFromPool(PoolEnums.Chute,Vector3.zero , Quaternion.identity, null).transform.GetChild(0).GetComponent<Chute>();
    //         chuteObj.meshCollider.enabled = true;
    //         chuteObj.meshRenderer.enabled = true;
    //         
    //         var mesh = chuteObj.meshFilter.mesh;
    //         mesh.Clear();
    //
    //         var upList = new List<Vector3>();
    //         var downList = new List<Vector3>();
    //         var forwardList = new List<Vector3>();
    //         var backList = new List<Vector3>();
    //         var finalList = new List<Vector3>();
    //
    //         Vector3 direction = new Vector3();
    //         for (int i = 1; i < pointsList.Count - 1; i++)
    //         {
    //             if (pointsList[i].x < pointsList[i+1].x)
    //             {
    //                 direction = Vector3.right;
    //             }
    //             else
    //             {
    //                 direction = Vector3.left;
    //             }
    //             
    //             backList.Add(pointsList[i] + Vector3.back + direction);
    //             upList.Add(pointsList[i] + Vector3.up + direction);
    //             forwardList.Add(pointsList[i] + Vector3.forward + direction);
    //             downList.Add(pointsList[i] + Vector3.down + direction);
    //         }
    //
    //         finalList.Add(pointsList[0]);
    //         for (int i = 0; i < upList.Count; i++)
    //         {
    //             finalList.Add(backList[i]);
    //             finalList.Add(upList[i]);
    //             finalList.Add(forwardList[i]);
    //             finalList.Add(downList[i]);
    //         }
    //         
    //         finalList.Add(pointsList[pointsList.Count-1] + (2f*direction));
    //
    //         // Assign the points to the mesh's vertices
    //         mesh.vertices = finalList.ToArray();
    //         mesh.uv = ConvertArray(mesh.vertices);
    //         
    //         List<int> triangles = new List<int>();
    //         for (int i = 0; i < 4; i++)
    //         {
    //             triangles.Add(0);
    //             triangles.Add(i + 2);
    //             triangles.Add(i + 1);
    //         }
    //
    //         triangles[10] = 1;
    //         
    //         if (backList.Count>1)
    //         {
    //             for (int i = 0; i < backList.Count-1; i++)
    //             {
    //                 triangles.Add((i*4)+1);
    //                 triangles.Add((i*4)+6);
    //                 triangles.Add((i*4)+5);
    //                 triangles.Add((i*4)+1);
    //                 triangles.Add((i*4)+2);
    //                 triangles.Add((i*4)+6);
    //                 
    //             }
    //             for (int i = 0; i < upList.Count-1; i++)
    //             {
    //                 triangles.Add((i*4)+2);
    //                 triangles.Add((i*4)+7);
    //                 triangles.Add((i*4)+6);
    //                 triangles.Add((i*4)+2);
    //                 triangles.Add((i*4)+3);
    //                 triangles.Add((i*4)+7);
    //                 
    //             }
    //             for (int i = 0; i < forwardList.Count-1; i++)
    //             {
    //                 triangles.Add((i*4)+3);
    //                 triangles.Add((i*4)+8);
    //                 triangles.Add((i*4)+7);
    //                 triangles.Add((i*4)+3);
    //                 triangles.Add((i*4)+4);
    //                 triangles.Add((i*4)+8);
    //                
    //             }
    //             for (int i = 0; i < downList.Count-1; i++)
    //             {
    //                 triangles.Add((i*4)+4);
    //                 triangles.Add((i*4)+5);
    //                 triangles.Add((i*4)+8);
    //                 triangles.Add((i*4)+4);
    //                 triangles.Add((i*4)+1);
    //                 triangles.Add((i*4)+5);
    //                 
    //             }
    //         }
    //         
    //         for (int i = finalList.Count-5; i < finalList.Count - 1; i++)
    //         {
    //             triangles.Add(finalList.Count - 1);
    //             triangles.Add(i);
    //             triangles.Add(i + 1);
    //         }
    //         
    //         triangles[triangles.Count - 1] = finalList.Count - 5;
    //         //triangles.Reverse();
    //         mesh.triangles = triangles.ToArray();
    //         mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
    //         mesh.RecalculateNormals();
    //
    //         chuteObj.meshFilter.mesh = mesh;
    //         chuteObj.meshCollider.sharedMesh = mesh;
    //
    //         var parent = chuteObj.transform.parent;
    //         chuteObj.transform.SetParent(null);
    //         parent.transform.position = chuteObj.meshCollider.bounds.center;
    //         chuteObj.transform.SetParent(parent);
    //         parent.transform.position = Vector3.zero;
    //     }
    // }
    //
    // Vector2[] ConvertArray(Vector3[] v3){
    //     Vector2 [] v2 = new Vector2[v3.Length];
    //     for(int i = 0; i <  v3.Length; i++){
    //         Vector3 tempV3 = v3[i];
    //         v2[i] = new Vector2(tempV3.x, tempV3.y);
    //     }
    //     return v2;
    // }
}
