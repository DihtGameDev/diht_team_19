using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTransform : MonoBehaviour
{

    private string moveInputAxis = "Vertical";
    private string turnInputAxis = "Horizontal";

    public float rotationRate = 360;
    public float moveSpeed = 500000;

    // Update is called once per frame
    void Update()
    {
        float moveAxis = Input.GetAxis(moveInputAxis);
        float turnAxis = Input.GetAxis(turnInputAxis);

        ApplyMovement(moveAxis, turnAxis);
    }

    private void ApplyMovement(float moveVertical, float moveHorizontal)
    {
        transform.Translate(Vector3.forward * moveVertical * moveSpeed * Time.deltaTime);
        transform.Translate(Vector3.right * moveHorizontal * moveSpeed * Time.deltaTime);
    }


    private void Turn(float input)
    {
        transform.Rotate(0, input * rotationRate * Time.deltaTime, 0);
    }
}
