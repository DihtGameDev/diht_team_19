using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class FoxController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log(name + " The fox was clicked! (⊙_⊙;)");
    }


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
    public float moveSpeed = 1;
    public float satiety = 50;
    public float hunger_rate = 1;
    public float starvation_threshold = 0;
    public float overeating_threshold = 110;
    public State state = State.Calm;
    public float target_search_delay = 0.5f;
    public GameObject target_marker_origin = null;
    public bool logging = false;
    
    void Start()
    {
        controller.Register(this);
        target_marker_ = target_marker_origin != null ? Instantiate(target_marker_origin) : null;
        target_ = transform.position;
        StartCoroutine("SearchForTarget");
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

    Vector3 GetDirection()
    {
        return (target_ - transform.position).normalized;
    }

    IEnumerator SearchForTarget()
    {
        while (true)
        {
            UpdateTarget();
            yield return new WaitForSeconds(target_search_delay);
        }
    }

    void UpdateTarget()
    {
        var old = target_;
        switch (state)
        {
            case State.Overate:
            case State.Calm:
                if ((target_ - transform.position).magnitude < 2)
                {
                    target_ = ChooseRandomTargetNear(transform.position, 20);
                }
                break;
            case State.Hungry:
                var position = transform.position;
                var food_source = GetClosestFoodSource();
                target_ = food_source.transform.position;
                break;
            case State.Frenzy:
                ChooseRandomTargetNear(transform.position, 20);
                break;
            default:
                break;

        }
        if (target_marker_ != null)
        {
            target_marker_.transform.SetPositionAndRotation(target_ + Vector3.up * 5, Quaternion.identity);
        }
        if (logging)
        {
            string msg = "Updated target from " + old + " to " + target_ + " in state " + state;
            Debug.Log(msg);
        }
    }

    Vector3 ChooseRandomTargetNear(Vector3 pivot, float maxDistance)
    {
        Vector3 random_target = Vector3.zero;
        float angle = UnityEngine.Random.Range(0, (float)Math.PI * 2);
        float magnitude = UnityEngine.Random.Range(0.01f, maxDistance);
        random_target = new Vector3(magnitude * (float)Math.Sin(angle), 0, magnitude * (float)Math.Cos(angle));
        return random_target + pivot;
    }

    void ChangeState()
    {
        switch (state)
        {
            case State.Calm:
                if (satiety < 30)
                {
                    if (logging)
                    {
                        Debug.Log("The fox is hungry!");
                    }
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

    void Die()
    {
        if (logging)
        {
            Debug.Log("The fox is dead :(");
        }
        StopAllCoroutines();
        Destroy(this);
    }

    FoodSourceBehaviour GetClosestFoodSource()
    {
        var position = transform.position;
        return controller.GetClosestFoodSource(position);
    }

    void TryEat()
    {
        if (state == State.Afraid || state == State.Frenzy || state == State.Overate)
        {
            return;
        }
        var food_source = GetClosestFoodSource();
        var position = transform.position;
        var food_position = food_source.transform.position;
        if ((food_position - position).magnitude < 10)
        {
            satiety += food_source.value * Time.deltaTime;
        }
    }
    private Vector3 target_ = Vector3.up;
    private FoodSourceBehaviour closest_food_source_;
    private GameObject target_marker_ = null;
}
