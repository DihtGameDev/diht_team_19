using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class FoodSourceBehaviour : Displayable
{
    private static long _id = 0;
    private int value = 10;
    private float amount = 100;
    private bool active = false;
    private long id = GetNextId();

    public long GetId()
    {
        return id;
    }
    private static long GetNextId()
    {
        return Interlocked.Increment(ref _id);
    }
    public string GetName()
    {
        return "Food source";
    }
    public void OnClick()
    {
        Debug.Log("Food source was clicked");
        GameController.Get().SetActive(this);
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
        GameController.Get().Register(this);
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
            GameController.Get().Unregister(this);
            gameObject.SetActive(false);
            return amount;
        }
    }
}
