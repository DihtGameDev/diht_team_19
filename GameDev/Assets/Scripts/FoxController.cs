using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxController : MonoBehaviour
{
    public enum State
    {
        Calm,
        Hungry,
        Afraid,
        Frenzy,
        Dead,
        Overate
    }

    public GameController controller;
    public float moveSpeed = 5;
    public float satiety = 50;
    public float hunger_rate = 1;
    public float starvation_threshold = 0;
    public float overeating_threshold = 110;
    public State state = State.Calm;
    
    void Start()
    {
        controller.Register(this);
    }
    
    void Update()
    {
        satiety -= hunger_rate * Time.deltaTime;
        
        ChangeState();
        Move();
        TryEat();
    }

    void Move()
    {
        Vector3 direction = GetDirection();
        var speed = state != State.Overate ? moveSpeed : moveSpeed * 0.7; 
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    void ChangeState()
    {
        switch (state)
        {
            case State.Calm:
                if (satiety < 30)
                {
                    Debug.Log("The fox is hungry!");
                    state = State.Hungry;
                } else if (satiety > overeating_threshold)
                {
                    state = State.Overate;
                }
                break;
            case State.Hungry:
                if (satiety > 70)
                {
                    state = State.Calm;
                } else if (satiety <= starvation_threshold)
                {
                    state = State.Dead;
                    Die();
                }
                break;
            case State.Overate:
                if (satiety < overeating_threshold)
                {
                    state = State.Calm;
                }
                break;
            case State.Afraid:
                break;
            case State.Frenzy:
                break;
            default:
                break;
        }
    }

    Vector3 GetDirection()
    {
        Vector3 direction = Vector3.zero;
        switch (state)
        {
            case State.Calm:
                direction.x = Random.Range(200, 700);
                direction.z = Random.Range(200, 700);
                direction = direction.normalized;
                break;
            case State.Hungry:
                var position = this.transform.position;
                var food_source = GetClosestFoodSource();
                var food_position = food_source.transform.position;
                direction = (food_position - position).normalized;
                break;
            case State.Afraid:
                break;
            case State.Frenzy:
                break;
            default:
                break;
        }
        return direction;
    }

    void Die()
    {
        Debug.Log("The fox is dead :(");
        Destroy(this);
    }

    FoodSourceBehaviour GetClosestFoodSource()
    {
        var position = this.transform.position;
        return controller.GetClosestFoodSource(position);
    }

    void TryEat()
    {
        if (state == State.Afraid || state == State.Frenzy || state == State.Overate)
        {
            return;
        }
        var food_source = GetClosestFoodSource();
        var position = this.transform.position;
        var food_position = food_source.transform.position;
        if ((food_position - position).magnitude < 10)
        {
            satiety += food_source.value * Time.deltaTime;
        }
    }

    FoodSourceBehaviour closestFoodSource;
}
