using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSourceBehaviour : Displayable
{
    public GameController controller;
    private int value = 10;
    private float amount = 100;
    private bool active = false;

    public string GetName()
    {
        return "Food source";
    }
    public void OnClick()
    {
        controller.SetActive(this);
        active = true;
    }

    public override void AfterClick()
    {
        active = false;
    }

    public override string GetTitle()
    {
        return GetName();
    }

    public override string GetInfo()
    {
        return "Eating rate: " + value.ToString();
    }

    public override string GetStatus()
    {
        return "Food left: " + amount.ToString();
    }

    private void Start()
    {
        controller.Register(this);
    }

    public float GetFood(float time)
    {
        var portion = time * value;
        if (portion < amount)
        {
            amount -= portion;
            return portion;
        } 
        else
        {
            controller.Unregister(this);
            return amount;
        }
    }
}
