using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BalloonHolder : Singleton<BalloonHolder>
{
    [HideInInspector] public List<Balloon> currentBalloons = new List<Balloon>();
    public Rigidbody rb { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AddToList(Balloon balloon)
    {
        currentBalloons.Add(balloon);
    }

    public void RemoveFromList(Balloon balloon)
    {
        currentBalloons.Remove(balloon);
    }
}
