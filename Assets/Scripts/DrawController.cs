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
    
    private bool canDraw, isDrawing;
    private Chute chuteObj;
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        canDraw = true;

        if (!isDrawing) return;
        if (pointsList.Count == 0) return;
        
        var startPoint = pointsList[pointsList.Count - 1];
        Draw(eventData.position);
        var endPoint = pointsList[pointsList.Count - 1];
        pointsList.RemoveAt(pointsList.Count - 1);
        lineRenderer.positionCount = pointsList.Count;
            
        float distance = Vector3.Distance(startPoint, endPoint);
        float pointDistance = 0.2f; 
        int numPoints = (int)(distance / pointDistance);
        float increment = 1.0f / (numPoints + 1);

        for (int i = 0; i < numPoints; i++) {
            float t = increment * (i + 1);
                
            Vector3 point = Vector3.Lerp(startPoint, endPoint, t);
            UpdatePointsList(point);
        }
    }
    

    public void OnPointerExit(PointerEventData eventData)
    {
        canDraw = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Draw(eventData.position);
        isDrawing = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Draw(eventData.position);
    }
    
    private void Draw(Vector3 position)
    {
        if (canDraw)
        {
            Vector3 touchPos = Camera.main.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, 10f)));

            UpdatePointsList(touchPos);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrawing = false;
        
        if(chuteObj != null) 
        {
            chuteObj.TryGetComponent<IPooledObject>(out var deactivateable);
            deactivateable?.OnObjectDeactivated();
        }
        CreateMesh();
        pointsList.Clear();
        lineRenderer.positionCount = 0;
    }
    
    public void CreateMesh()
    {
        if (pointsList.Count >= 3)
        {
            chuteObj = ObjectPool.Instance.SpawnFromPool(PoolEnums.Chute, Vector3.zero, Quaternion.identity, null).GetComponent<Chute>();
            Mesh mesh = new Mesh();

            var upList = new List<Vector3>();
            var downList = new List<Vector3>();
            var rightList = new List<Vector3>();
            var leftList = new List<Vector3>();
            var finalList = new List<Vector3>();
            
            for (int i = 1; i < pointsList.Count - 1; i++)
            {
                upList.Add(pointsList[i] + (Vector3.up * 0.1f * (i/(float)pointsList.Count)));
                downList.Add(pointsList[i] + (Vector3.down * 0.1f * (i/(float)pointsList.Count)));
                rightList.Add(pointsList[i] + (Vector3.right * 0.1f * (i/(float)pointsList.Count)));
                leftList.Add(pointsList[i] + (Vector3.left * 0.1f * (i/(float)pointsList.Count)));
            }

            finalList.Add(pointsList[0]);
            for (int i = 0; i < upList.Count; i++)
            {
                finalList.Add(upList[i]);
                finalList.Add(downList[i]);
                finalList.Add(rightList[i]);
                finalList.Add(leftList[i]);
            }
            finalList.Add(pointsList[pointsList.Count-1]);
            
            // Assign the points to the mesh's vertices
            mesh.vertices = finalList.ToArray();
            int[] triangles = new int[upList.Count * 8];
            int triangleIndex = 0;

            // First generate the middle set of triangles 
            for (int i = 1; i < pointsList.Count/2; i++)
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = i;
                triangles[triangleIndex++] = i + 1;
            }
            // Then generate the second half of the triangles 
            for (int i = pointsList.Count / 2 +1; i < pointsList.Count-1; i++)
            {
                triangles[triangleIndex++] = i;
                triangles[triangleIndex++] = i + 1;
                triangles[triangleIndex++] = pointsList.Count-1;
            }
            mesh.triangles = triangles;
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
            
            chuteObj.meshFilter.mesh = mesh;
            chuteObj.meshCollider.sharedMesh = mesh;
        }
    }

    private void UpdatePointsList(Vector3 position)
    {
        pointsList.Add(position);
        lineRenderer.positionCount = pointsList.Count;
        lineRenderer.SetPositions(pointsList.ToArray());
    }
}
