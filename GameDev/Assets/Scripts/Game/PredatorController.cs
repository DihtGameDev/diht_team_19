using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PredatorController : MonoBehaviour
{
    private bool active;
    private FoodSourceBehaviour closest_food_source_;

    private GameController controller;

    public PredatorData data;
    private float hunger_rate;
    private bool logging;
    private float moveSpeed;
    private float overeating_threshold;

    public int owner = -1;
    private float satiety;
    private float starvation_threshold;
    private AnimalState state;
    private Vector3 target_ = Vector3.up;

    [SerializeField] public GameObject target_marker;

    private float target_search_delay;

    public void OnClick()
    {
        Debug.Log(GetName() + " The fox was clicked! (⊙_⊙;)");
        controller.SetActive(this);
        active = true;
        target_marker.SetActive(true);
        UpdateTargetMarker();
    }

    public void AfterClick()
    {
        active = false;
        target_marker.SetActive(false);
    }

    public virtual string GetName()
    {
        return "Predator";
    }

    public AnimalState GetState()
    {
        return state;
    }

    public float GetSatiety()
    {
        return satiety;
    }


    private void InitVariables()
    {
        moveSpeed = data.moveSpeed;
        satiety = data.satiety;
        hunger_rate = data.hunger_rate;
        starvation_threshold = data.starvation_threshold;
        overeating_threshold = data.overeating_threshold;
        state = data.state;
        target_search_delay = data.target_search_delay;
        logging = data.logging;
        controller = GameController.Get();
    }

    private void Start()
    {
        InitVariables();
        target_ = transform.position;
        target_marker.transform.SetParent(controller.transform, true);
        StartCoroutine("SearchForTarget");
    }

    private void Update()
    {
        satiety -= hunger_rate * Time.deltaTime;

        ChangeState();
        Move();
        TryEat();
    }

    private void Move()
    {
        var direction = GetDirection();
        var speed = state != AnimalState.Overate ? moveSpeed : moveSpeed * 0.7;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    private Vector3 GetDirection()
    {
        return (target_ - transform.position).normalized;
    }

    private IEnumerator SearchForTarget()
    {
        while (true)
        {
            UpdateTarget();
            yield return new WaitForSeconds(target_search_delay);
        }
    }

    private void UpdateTarget()
    {
        var old = target_;
        switch (state)
        {
            case AnimalState.Overate:
            case AnimalState.Calm:
                if ((target_ - transform.position).magnitude < 2)
                    target_ = ChooseRandomTargetNear(transform.position, 20);
                break;
            case AnimalState.Hungry:
                var position = transform.position;
                var food_source = GetClosestFoodSource();
                target_ = food_source.transform.position;
                break;
            case AnimalState.Frenzy:
                ChooseRandomTargetNear(transform.position, 20);
                break;
        }

        UpdateTargetMarker();

        if (logging)
        {
            var msg = "Updated target from " + old + " to " + target_ + " in state " + state;
            Debug.Log(msg);
        }
    }

    private void UpdateTargetMarker()
    {
        if (active) target_marker.transform.SetPositionAndRotation(target_ + Vector3.up * 5, Quaternion.identity);
    }

    private Vector3 ChooseRandomTargetNear(Vector3 pivot, float maxDistance)
    {
        var random_target = Vector3.zero;
        var angle = Random.Range(0, (float) Math.PI * 2);
        var magnitude = Random.Range(0.01f, maxDistance);
        random_target = new Vector3(magnitude * (float) Math.Sin(angle), 0, magnitude * (float) Math.Cos(angle));
        return random_target + pivot;
    }

    private void ChangeState()
    {
        switch (state)
        {
            case AnimalState.Calm:
                if (satiety < 30)
                {
                    if (logging) Debug.Log("The fox is hungry!");
                    state = AnimalState.Hungry;
                }
                else if (satiety > overeating_threshold)
                {
                    state = AnimalState.Overate;
                }

                break;
            case AnimalState.Hungry:
                if (satiety > 70)
                {
                    state = AnimalState.Calm;
                }
                else if (satiety <= starvation_threshold)
                {
                    state = AnimalState.Dead;
                    Die();
                }

                break;
            case AnimalState.Overate:
                if (satiety < overeating_threshold) state = AnimalState.Calm;
                break;
            case AnimalState.Afraid:
                break;
            case AnimalState.Frenzy:
                break;
            case AnimalState.Dead:
                break;
        }
    }

    private void Die()
    {
        if (logging) Debug.Log("The fox is dead :(");
        StopAllCoroutines();
        if (owner >= 0) controller.Unregister(this, owner);

        Destroy(this);
    }

    private FoodSourceBehaviour GetClosestFoodSource()
    {
        var position = transform.position;
        return controller.GetClosestFoodSource(position);
    }

    private void TryEat()
    {
        if (state == AnimalState.Afraid || state == AnimalState.Frenzy || state == AnimalState.Overate) return;
        var food_source = GetClosestFoodSource();
        var position = transform.position;
        var food_position = food_source.transform.position;
        if ((food_position - position).magnitude < 10) satiety += food_source.value * Time.deltaTime;
    }
}