using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Balloon : MonoBehaviour,IPooledObject
{
    [SerializeField] private Transform anchorPos;
    [SerializeField] private float upwardForce = 15f;
    public SpringJoint springJoint { get; private set; }
    
    private Rigidbody rb;
    private LineRenderer lineRenderer;
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
        
        //Do scaling effect
        transform.localScale = Vector3.one * 0.01f;
        transform.DOScale(Vector3.one, 1f).OnComplete(() =>
        {
            collider.enabled = true;
        });
        
        //Change color and add to the holder
        meshRenderer.material.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f);
        BalloonHolder.Instance.AddToList(this);
    }

    public void OnObjectDeactivated()
    {
        
    }

    private void FixedUpdate()
    {
        //Upwards movement
        rb.AddForce(Vector3.up * upwardForce, ForceMode.Acceleration);
    }

    private void Update()
    {
        //Smoothly rotate towards the holder position
        var targetPos = BalloonHolder.Instance.transform.position;
        Vector3 relativePos = targetPos - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, 3f * Time.deltaTime);

        //Update line renderer
        lineRenderer.SetPosition(0,anchorPos.position);
        lineRenderer.SetPosition(1,targetPos);
    }
}
