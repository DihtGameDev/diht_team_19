using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSourceBehaviour : MonoBehaviour
{
    public GameController controller;
    public int value = 10;

    void Start()
    {
        controller.Register(this);
    }

    void Update()
    {
        
    }
}
