using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7;
    [SerializeField] private float smoothMoveTime = 0.1f;
    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private float maxXDistance;

    //private float angle;
    private Vector3 inputDirection;
    private float smoothInputMagnitude;
    private float inputMagnitude;
    private Vector3 velocity;
    private Joystick joystick;
    private Rigidbody rb;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        joystick = Joystick.Instance;
    }

    private void FixedUpdate()
    {
        inputDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;
        inputMagnitude = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;
        
        //float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        //angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
        smoothInputMagnitude = Mathf.Lerp(smoothInputMagnitude, inputMagnitude, smoothMoveTime);
        velocity = inputDirection * moveSpeed * smoothInputMagnitude;
        
        Vector3 positionAtEndOfStep = rb.position + velocity * Time.deltaTime;
        positionAtEndOfStep.x = Mathf.Clamp(positionAtEndOfStep.x, -maxXDistance, maxXDistance);

        //rb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rb.MovePosition(positionAtEndOfStep);
    }
}