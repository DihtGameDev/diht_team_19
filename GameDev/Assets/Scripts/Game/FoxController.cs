using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class FoxController : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log(name + " The fox was clicked! (⊙_⊙;)");
    }

    public PredatorData data;

    private float moveSpeed;
    private float satiety;
    private float hunger_rate;
    private float starvation_threshold;
    private float overeating_threshold;
    private AnimalState state;
    private float target_search_delay;
    private bool logging;

    [SerializeField]
    public GameController controller;
    [SerializeField]
    public GameObject target_marker_origin = null;

    void initVariables()
    {
        moveSpeed = data.moveSpeed;
        satiety = data.satiety;
        hunger_rate = data.hunger_rate;
        starvation_threshold = data.starvation_threshold;
        overeating_threshold = data.overeating_threshold;
        state = data.state;
        target_search_delay = data.target_search_delay;
        target_marker_origin = target_marker_origin;
        logging = data.logging;
}

    void Start()
    {
        initVariables();
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
        var speed = state != AnimalState.Overate ? moveSpeed : moveSpeed * 0.7; 
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
            case AnimalState.Overate:
            case AnimalState.Calm:
                if ((target_ - transform.position).magnitude < 2)
                {
                    target_ = ChooseRandomTargetNear(transform.position, 20);
                }
                break;
            case AnimalState.Hungry:
                var position = transform.position;
                var food_source = GetClosestFoodSource();
                target_ = food_source.transform.position;
                break;
            case AnimalState.Frenzy:
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
            case AnimalState.Calm:
                if (satiety < 30)
                {
                    if (logging)
                    {
                        Debug.Log("The fox is hungry!");
                    }
                    state = AnimalState.Hungry;
                } else if (satiety > overeating_threshold)
                {
                    state = AnimalState.Overate;
                }
                break;
            case AnimalState.Hungry:
                if (satiety > 70)
                {
                    state = AnimalState.Calm;
                } else if (satiety <= starvation_threshold)
                {
                    state = AnimalState.Dead;
                    Die();
                }
                break;
            case AnimalState.Overate:
                if (satiety < overeating_threshold)
                {
                    state = AnimalState.Calm;
                }
                break;
            case AnimalState.Afraid:
                break;
            case AnimalState.Frenzy:
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
        if (state == AnimalState.Afraid || state == AnimalState.Frenzy || state == AnimalState.Overate)
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
