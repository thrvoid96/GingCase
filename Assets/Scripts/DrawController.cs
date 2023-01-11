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
        //CreateMesh();
        pointsList.Clear();
        //lineRenderer.positionCount = 0;
    }
    
    public void CreateMesh()
    {
        if (pointsList.Count > 0)
        {
            chuteObj = ObjectPool.Instance.SpawnFromPool(PoolEnums.Chute, Vector3.zero, Quaternion.identity, null).GetComponent<Chute>();

            Mesh mesh = new Mesh();
            mesh.SetVertices(pointsList);
            mesh.SetColors(Enumerable.Repeat(Color.black, pointsList.Count).ToList());
            mesh.SetIndices(Enumerable.Range(0, pointsList.Count).ToArray(), MeshTopology.LineStrip, 0);
            
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
