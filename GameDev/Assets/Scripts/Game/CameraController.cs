using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private string upInputAxis = "Vertical";
    private string sideInputAxis = "Horizontal";

    public float moveSpeed = 20;
    public float zoomSpeed = 100;
    public float zoomLowLimit = 10.0f;
    public float zoomHighLimit = 100.0f;
    
    void Start()
    {
        
    }

    void Update()
    {
        float moveAxis = Input.GetAxis(upInputAxis);
        float turnAxis = Input.GetAxis(sideInputAxis);
        float zoomValue = Input.mouseScrollDelta.y;

        ApplyMovement(moveAxis, turnAxis);
        ApplyZoom(zoomValue);
    }

    void ApplyMovement(float moveVertical, float moveHorizontal)
    {
        transform.Translate(Vector3.up * (moveVertical * moveSpeed * Time.deltaTime));
        transform.Translate(Vector3.right * (moveHorizontal * moveSpeed * Time.deltaTime));
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
