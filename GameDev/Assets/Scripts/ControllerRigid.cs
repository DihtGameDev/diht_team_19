using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerRigid : MonoBehaviour
{

    private string moveInputAxis = "Vertical";
    private string turnInputAxis = "Horizontal";

    public float rotationRate = 360;
    public float moveSpeed = 500000;
    public float gravity = -200;

    [SerializeField]
    private Rigidbody rb;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveAxis = Input.GetAxis(moveInputAxis);
        float turnAxis = Input.GetAxis(turnInputAxis);

        ApplyMovement(moveAxis, turnAxis);
    }

    private void ApplyMovement(float moveVertical, float moveHorizontal)
    {

        rb.AddForce(Vector3.forward * moveVertical * moveSpeed * Time.deltaTime, ForceMode.Force);
        rb.AddForce(Vector3.right * moveHorizontal * moveSpeed * Time.deltaTime, ForceMode.Force);
        rb.AddForce(Vector3.up * gravity * Time.deltaTime, ForceMode.Force);
    }


    private void Turn(float input)
    {
        transform.Rotate(0, input * rotationRate * Time.deltaTime, 0);
    }
}
