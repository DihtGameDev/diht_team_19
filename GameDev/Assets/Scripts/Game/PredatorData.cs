using UnityEngine;

public enum AnimalState
{
    Calm,
    Hungry,
    Afraid,
    Frenzy,
    Dead,
    Overate,
    Eating
}

[CreateAssetMenu(
    fileName = "New PredatorData", 
    menuName = "Predator Data", 
    order = 51)]
public class PredatorData : ScriptableObject
{
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
