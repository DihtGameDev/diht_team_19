using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Animals;
using UnityEngine;
using Random = UnityEngine.Random;

public class PredatorController : Displayable
{
    private bool active;
    private FoodSourceBehaviour closest_food_source_;

    private GameController controller;

    public List<Ability> battleAbilities;
    public List<Ability> peacefulAbilities;
    private PredatorData data;
    private float hunger_rate;
    private bool logging;
    private float moveSpeed;
    private float overeating_threshold;
    public int aggression;
    public float health;
    public float armor;
    public float attackPower;
    public float energy;
    private float attackRange;
    public int owner = -1;
    private float satiety;
    private float starvation_threshold;
    private AnimalState state;
    private Vector3 target_ = Vector3.up;
    public delegate void AdditionalLogic(PredatorController a);

    private AdditionalLogic logic = (predator) => {  };

    public bool IsActive()
    {
        return active;
    }
    public void AddLogic(AdditionalLogic l)
    {
        logic += l;
    }
    
    private IEnumerator CalmDown()
    {
        var old_trans = afraid_of;
        yield return new WaitForSeconds(5);
        if (state == AnimalState.Afraid && old_trans == afraid_of)
        {
            state = AnimalState.Calm;
        }
    }
    public void Scare(PredatorController source)
    {
        state = AnimalState.Afraid;
        afraid_of = source.transform;
        StartCoroutine("CalmDown");
    }
    
    [SerializeField] public GameObject target_marker;

    private float target_search_delay;

    public void SetMoveSpeed(float val)
    {
        moveSpeed = val;
    }

    public void SetHungerRate(float val)
    {
        hunger_rate = val;
    }
    public void OnClick()
    {
        Debug.Log(GetName() + " was clicked! (⊙_⊙;)");
        controller.SetActive(this);
        active = true;
        target_marker.SetActive(true);
        UpdateTargetMarker();
    }

    public override void AfterClick()
    {
        active = false;
        target_marker.SetActive(false);
    }

    public override string GetTitle()
    {
        return GetName();
    }

    public override string GetInfo()
    {
        return "Satiety " + satiety.ToString("0") + "\nHealth " + health.ToString("0");
    }

    public override string GetStatus()
    {
        return GetStateString(state);
    }

    public virtual string GetName()
    {
        return "Predator";
    }

    private string GetStateString(AnimalState state)
    {
        switch (state)
        {
            case AnimalState.Calm:
                return "Calm";
            case AnimalState.Hungry:
                return "Hungry";
            case AnimalState.Afraid:
                return "Afraid";
            case AnimalState.Frenzy:
                return "Frenzy";
            case AnimalState.Dead:
                return "Dead";
            case AnimalState.Overate:
                return "Overate";
            case AnimalState.Eating:
                return "Eating";
            case AnimalState.Fighting:
                return "Fighting";
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }   
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
        controller = GameController.Get();
        data = controller.players[owner].data;
        moveSpeed = data.moveSpeed;
        satiety = data.satiety;
        hunger_rate = data.hunger_rate;
        starvation_threshold = data.starvation_threshold;
        overeating_threshold = data.overeating_threshold;
        state = data.state;
        target_search_delay = data.target_search_delay;
        logging = data.logging;
        aggression = data.aggression;
        health = data.health;
        attackPower = data.attackPower;
        attackRange = data.attackRange;
        armor = data.armor;
        battleAbilities = data.battleAbilities;
        peacefulAbilities = data.peacefulAbilities;
        energy = data.energy;
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
        TryFight();
        logic(this);
    }

    private List<AnimalState> nonMovable = new List<AnimalState> {AnimalState.Dead, AnimalState.Eating};
    private void Move()
    {
        if (nonMovable.Contains(state))
        {
            return;
        }
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
                if (food_source == null)
                {
                    target_ = ChooseRandomTargetNear(transform.position, 10);
                    break;
                }
                target_ = food_source.transform.position;
                break;
            case AnimalState.Frenzy:
                ChooseRandomTargetNear(transform.position, 20);
                break;
            case AnimalState.Afraid:
                if (afraid_of != null) target_ = (2 * transform.position - afraid_of.transform.position);
                break;
            case AnimalState.Dead:
                break;
            case AnimalState.Eating:
                break;
            case AnimalState.Fighting:
                var enemy = GetEnemy();
                target_ = enemy.transform.position;
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
        return GameController.GetPlaceNear(pivot, 0.01f, maxDistance);
    }

    private void ChangeState()
    {
        switch (state)
        {
            case AnimalState.Calm:
                if (satiety < 30)
                {
                    if (logging) Debug.Log("The " + GetName() + " is hungry!");
                    state = AnimalState.Hungry;
                }
                else if (satiety > overeating_threshold)
                {
                    state = AnimalState.Overate;
                } else if (Random.Range(0, 5000) <= aggression)
                {
                    state = AnimalState.Fighting;
                }
                break;
            case AnimalState.Hungry:
                var food_source = GetClosestFoodSource();
                var position = transform.position;
                
                if ((food_source != null) && (food_source.transform.position - position).magnitude < 4)
                {
                    state = AnimalState.Eating;
                }
                break;
            case AnimalState.Overate:
                if (satiety < overeating_threshold) state = AnimalState.Calm;
                break;
            case AnimalState.Afraid:
                if (afraid_of == null || (transform.position - afraid_of.position).magnitude > 15)
                {
                    afraid_of = null;
                    state = AnimalState.Calm;
                    UpdateTarget();
                }
                break;
            case AnimalState.Frenzy:
                break;
            case AnimalState.Dead:
                break;
            case AnimalState.Eating:
                if (satiety > 70)
                {
                    state = AnimalState.Calm;
                }
                break;
            case AnimalState.Fighting:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (satiety <= starvation_threshold)
        {
            state = AnimalState.Dead;
            Die();
        }
    }

    private void Die()
    {
        if (logging) Debug.Log("The fox is dead :(");
        StopAllCoroutines();
        if (owner >= 0) controller.Unregister(this, owner);
        gameObject.SetActive(false);
    }

    private FoodSourceBehaviour GetClosestFoodSource()
    {
        var position = transform.position;
        return controller.GetClosestFoodSource(position);
    }
    
    private PredatorController current_enemy = null;
    private Transform afraid_of = null;
    private PredatorController GetEnemy()
    {
        if (current_enemy != null) return current_enemy;
        var position = transform.position;
        current_enemy = controller.GetClosestEnemy(position, owner);
        return current_enemy;
    }

    private void TryEat()
    {
        if (state != AnimalState.Eating) return;
        var food_source = GetClosestFoodSource();
        if (food_source == null)
        {
            state = AnimalState.Hungry;
            return;
        }
        var position = transform.position;
        var food_position = food_source.transform.position;
        if ((food_position - position).magnitude < 10) satiety += food_source.GetFood(Time.deltaTime);
        else state = AnimalState.Calm;
    }

    private void TryBattleAbility(PredatorController enemy)
    {
        var indexes = Enumerable.Range(0, battleAbilities.Count).ToList();
        var rnd = GameController.rnd;
        indexes = indexes.OrderBy(x => rnd.Next()).ToList();
        foreach (var ind in indexes)
        {
            if (battleAbilities[ind].Available(this, enemy))
            {
                battleAbilities[ind].Apply(this, enemy);
                break;
            }
        }
    }
    private void TryFight()
    {
        if (state != AnimalState.Fighting) return;
        var enemy = GetEnemy();
        if (Random.Range(0, 100) <= 2) TryBattleAbility(enemy);
        if (enemy == null || !enemy.gameObject.activeSelf)
        {
            state = AnimalState.Calm;
            return;
        }

        if (!ready) return;
        var position = transform.position;
        var enemy_position = enemy.transform.position;
        if ((enemy_position - position).magnitude < attackRange) Attack(enemy);
        ready = false;
        StartCoroutine("Reload");

    }

    private void Damage(PredatorController source)
    {
        health -= source.attackPower * (1 - armor);
        if (health <= 0)
        {
            var controller = GameController.Get();
            controller.players[source.owner].points += 12;
            controller.updatePointInfo();
            Die();
        }
        else if (health < 10) Scare(source);
        state = AnimalState.Fighting;
        current_enemy = source;
    }

    private bool ready = true;
    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(3);
        ready = true;
    }
    private void Attack(PredatorController enemy)
    {
        Debug.Log("Attacked for " + attackPower * (1 - armor));
        enemy.Damage(this);
    }
}