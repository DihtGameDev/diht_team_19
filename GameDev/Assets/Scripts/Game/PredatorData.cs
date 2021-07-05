using System.Collections.Generic;
using Game.Animals;
using UnityEngine;

public enum AnimalState
{
    Calm,
    Hungry,
    Afraid,
    Frenzy,
    Dead,
    Overate,
    Eating,
    Fighting
}

[CreateAssetMenu(
    fileName = "New PredatorData", 
    menuName = "Predator Data", 
    order = 51)]
public class PredatorData : ScriptableObject
{
    public List<Ability> battleAbilities = new List<Ability>();
    public List<Ability> peacefulAbilities = new List<Ability>();

    [SerializeField] 
    public float energy = 0.0f;
    [SerializeField]
    public float armor = 0.0f;
    [SerializeField]
    public float attackPower = 4;
    [SerializeField]
    public float attackRange = 4;
    [SerializeField]
    public int aggression = 1;
    [SerializeField]
    public float health = 20;
    [SerializeField]
    public float moveSpeed = 2;
    [SerializeField]
    public float satiety = 50;
    [SerializeField]
    public float hunger_rate = 1;
    [SerializeField]
    public float starvation_threshold = 0;
    [SerializeField]
    public float overeating_threshold = 110;
    [SerializeField]
    public AnimalState state = AnimalState.Calm;
    [SerializeField]
    public float target_search_delay = 0.5f;
    [SerializeField]
    public bool logging = false;
}
