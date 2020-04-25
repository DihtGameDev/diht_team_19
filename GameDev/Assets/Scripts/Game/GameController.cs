using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public void Pause()
    {
        Debug.Log("Pausing the game...");
    }

    public void Play()
    {
        Debug.Log("Resuming the game...");
    }

    public void Register(FoxController fox)
    {
        foxes.Add(fox);
    }

    public void Register(FoodSourceBehaviour source)
    {
        foodSources.Add(source);
    }

    public FoodSourceBehaviour GetClosestFoodSource(Vector3 position)
    {
        double min_distance = double.PositiveInfinity;
        FoodSourceBehaviour answer = null;

        foreach (var source in foodSources)
        {
            var cur_distance = Vector3.Distance(position, source.transform.position);
            if (cur_distance < min_distance)
            {
                answer = source;
                min_distance = cur_distance;
            }
        }
        return answer;
    }

    void Start()
    {
        Debug.Log("The game has started!");
    }

    void Update()
    {
        
    }

    private List<FoxController> foxes = new List<FoxController>();
    private List<FoodSourceBehaviour> foodSources = new List<FoodSourceBehaviour>();
}
