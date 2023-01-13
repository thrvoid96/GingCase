using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BalloonHolder : Singleton<BalloonHolder>
{
    private List<Balloon> currentBalloons = new List<Balloon>();
    public List<Balloon> getCurrentBalloonsList => currentBalloons;
    
    private Rigidbody rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AddToList(Balloon balloon)
    {
        currentBalloons.Add(balloon);
        balloon.springJoint.connectedBody = rb;
    }

    public void RemoveFromList(Balloon balloon)
    {
        currentBalloons.Remove(balloon);
    }
}
