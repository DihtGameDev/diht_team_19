using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const string UpInputAxis = "Vertical";
    private const string SideInputAxis = "Horizontal";

    public float moveSpeed = 20;
    public float zoomSpeed = 100;
    public float zoomLowLimit = 10.0f;
    public float zoomHighLimit = 100.0f;
    
    void Update()
    {
        float verticalAxis = Input.GetAxis(UpInputAxis);
        float horizontalAxis = Input.GetAxis(SideInputAxis);
        float zoomValue = Input.mouseScrollDelta.y;

        ApplyMovement(verticalAxis, horizontalAxis);
        ApplyZoom(zoomValue);
    }

    void ApplyMovement(float moveVertical, float moveHorizontal)
    {
        var forw = transform.forward;
        forw.y = 0;
        forw = transform.InverseTransformDirection(forw);
        forw.Normalize();
        
        transform.Translate(forw * (moveVertical * moveSpeed * Time.deltaTime));
        transform.Translate(transform.right * (moveHorizontal * moveSpeed * Time.deltaTime));
    }

    void ApplyZoom(float value)
    {
        if (transform.position.y >= zoomHighLimit && value < 0 ||
            transform.position.y <= zoomLowLimit && value > 0)
        {
            return;
        }
        transform.Translate(Vector3.forward * (value * zoomSpeed * Time.deltaTime));
    }
}
