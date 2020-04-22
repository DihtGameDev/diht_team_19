using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private string upInputAxis = "Vertical";
    private string sideInputAxis = "Horizontal";

    public float moveSpeed = 20;

    void Start()
    {
        
    }

    void Update()
    {
        float moveAxis = Input.GetAxis(upInputAxis);
        float turnAxis = Input.GetAxis(sideInputAxis);

        ApplyMovement(moveAxis, turnAxis);
    }

    void ApplyMovement(float moveVertical, float moveHorizontal)
    {
        transform.Translate(Vector3.up * moveVertical * moveSpeed * Time.deltaTime);
        transform.Translate(Vector3.right * moveHorizontal * moveSpeed * Time.deltaTime);
    }
}
