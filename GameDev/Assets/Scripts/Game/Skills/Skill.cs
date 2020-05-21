using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public string GetName()
    {
        return name;
    }

    public string GetDescription()
    {
        return description;
    }

    public uint GetPrice()
    {
        return price;
    }
    
    public List<Skill> GetNext()
    {
        return next;
    }

    public void activate(int player_id)
    {
        act(player_id);
    }

    public static Skill hunger(int iteration)
    {
        if (iteration > 3)
        {
            return null;
        }
        return new Skill("Hunger " + iteration, "Your creatures can live without food longer", 10, new List<Skill>{hunger(iteration + 1)},  (int player_id) =>
        {
            const float min_hunger_rate = 0.7f;
            var player = GameController.Get().players[player_id];
            player.data.hunger_rate = Math.Max(player.data.hunger_rate - 0.1f, min_hunger_rate);
            foreach (var pr in player.predators)
            {
                pr.SetHungerRate(player.data.hunger_rate);
            }
            if (player.data.logging) Debug.Log("Improved hunger rate");
        }); 
    }
    public static Skill speed(int iteration)
    {
        if (iteration > 3)
        {
            return null;
        }
        return new Skill("Speed " + iteration, "Increase speed of all your creatures", 10, new List<Skill>{speed(iteration + 1)},  (int player_id) =>
        {
            const float max_movemenet_speed = 15;
            var player = GameController.Get().players[player_id];
            player.data.moveSpeed = Math.Min(player.data.moveSpeed + 1, max_movemenet_speed);
            foreach (var pr in player.predators)
            {
                pr.SetMoveSpeed(player.data.moveSpeed);
            }
            if (player.data.logging) Debug.Log("Improved speed");
        });
    }

    public static readonly Skill initial = new Skill("", "", 0, new List<Skill> {speed(1), hunger(1)}, (int pid) => { });
    
    private Skill(string name, string description, uint price, List<Skill> next, activator act)
    {
        this.name = name;
        this.description = description;
        this.act = act;
        this.next = next;
    }

    delegate void activator(int player_id);
    private activator act;
    private uint price;
    private List<Skill> next;
    private string name;
    private string description;
}



