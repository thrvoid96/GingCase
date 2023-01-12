using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Balloon : MonoBehaviour,IPooledObject
{
    public float upwardForce = 10f;
    [SerializeField] private Transform anchorPos;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private SpringJoint springJoint;
    private MeshRenderer meshRenderer;
    private Collider collider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        springJoint = GetComponent<SpringJoint>();
        collider = GetComponent<Collider>();
        
        lineRenderer = transform.GetChild(0).GetComponent<LineRenderer>();
        meshRenderer = transform.GetChild(1).GetComponent<MeshRenderer>();
        
    }

    public void OnObjectSpawn()
    {
        collider.enabled = false;
        
        transform.localScale = Vector3.one * 0.01f;
        transform.DOScale(Vector3.one, 1f).OnComplete(() =>
        {
            collider.enabled = true;
        });
        
        meshRenderer.material.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f);
        springJoint.connectedBody = BalloonHolder.Instance.rb;
        BalloonHolder.Instance.AddToList(this);
    }

    public void OnObjectDeactivated()
    {
        
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.up * upwardForce, ForceMode.Acceleration);
        transform.LookAt(BalloonHolder.Instance.transform.position,Vector3.forward);
        lineRenderer.SetPosition(0,anchorPos.position);
        lineRenderer.SetPosition(1,BalloonHolder.Instance.transform.position);
    }
}
