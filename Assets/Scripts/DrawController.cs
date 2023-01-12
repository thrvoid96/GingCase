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
            var mesh = chuteObj.meshFilter.mesh;
            mesh.Clear();

            var upList = new List<Vector3>();
            var downList = new List<Vector3>();
            var forwardList = new List<Vector3>();
            var backList = new List<Vector3>();
            var finalList = new List<Vector3>();
            
            for (int i = 1; i < pointsList.Count - 1; i++)
            {
                backList.Add(pointsList[i] + Vector3.back + Vector3.right);
                upList.Add(pointsList[i] + Vector3.up + Vector3.right);
                forwardList.Add(pointsList[i] + Vector3.forward + Vector3.right);
                downList.Add(pointsList[i] + Vector3.down + Vector3.right);
            }

            finalList.Add(pointsList[0]);
            for (int i = 0; i < upList.Count; i++)
            {
                finalList.Add(forwardList[i]);
                finalList.Add(upList[i]);
                finalList.Add(backList[i]);
                finalList.Add(downList[i]);
            }
            finalList.Add(pointsList[pointsList.Count-1] + Vector3.right);
            
            // Assign the points to the mesh's vertices
            mesh.vertices = finalList.ToArray();
            mesh.uv = ConvertArray(mesh.vertices);
            
            int[] triangles = new int[upList.Count * 8 * 3];
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                triangles[index] = 0;
                triangles[index + 1] = i + 1;
                triangles[index + 2] = i + 2;
                index += 3;
            }

            triangles[11] = 1;
            
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
    
    Vector2[] ConvertArray(Vector3[] v3){
        Vector2 [] v2 = new Vector2[v3.Length];
        for(int i = 0; i <  v3.Length; i++){
            Vector3 tempV3 = v3[i];
            v2[i] = new Vector2(tempV3.x, tempV3.y);
        }
        return v2;
    }
}
