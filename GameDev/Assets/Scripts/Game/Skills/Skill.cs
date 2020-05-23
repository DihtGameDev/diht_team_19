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

    public int GetPrice()
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
        List<Skill> next;
        if (iteration > 3)
        {
            next = new List<Skill>();
        }
        else
        {
            next = new List<Skill> {hunger(iteration + 1)};
        }
        return new Skill("Hunger " + iteration, "Your creatures can live without food longer", 10, next,  (int player_id) =>
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
        List<Skill> next;
        if (iteration > 3)
        {
            next = new List<Skill>();
        }
        else
        {
            next = new List<Skill> {speed(iteration + 1)};
        }
        return new Skill("Speed " + iteration, "Increase speed of all your creatures", 10, next,  (int player_id) =>
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
    
    public static readonly Skill foxUnique = new Skill("Fox unique", "Unique fox skill", 0, new List<Skill>{foxUnique2}, (int pid) => {});

    public static readonly Skill foxUnique2 = new Skill("Fox unique 2", "Second unique fox skill", 0, new List<Skill>(), (int pid) => {});

    public static Skill wolfUnique(int iteration)
    {
        List<Skill> next;
        if (iteration > 5)
        {
            next = new List<Skill>();
        }
        else
        {
            next = new List<Skill> {wolfUnique(iteration + 1)};
        }
        
        return new Skill("Engineering " + iteration, "Increase engineering ability", 12, next,  (int player_id) =>
        {
            PredatorController.AdditionalLogic l = (wolf) =>
            {
                if (wolf.IsActive())
                {
                    Debug.Log("ADDITIONAL WOLF WAS CLICKED!!!");
                }
            }; 

            var player = GameController.Get().players[player_id];
            foreach (var pr in player.predators)
            {
                pr.AddLogic(l);
            }
            if (player.data.logging) Debug.Log("Added logic!");
        });
    }
    
    public static readonly Skill initial = new Skill("", "", 0, new List<Skill> {speed(1), hunger(1)}, (int pid) => { });
    
    private Skill(string name, string description, int price, List<Skill> next, activator act)
    {
        this.name = name;
        this.description = description;
        this.act = act;
        this.next = next;
        this.price = price;
    }

    delegate void activator(int player_id);
    private activator act;
    private int price;
    private List<Skill> next;
    private string name;
    private string description;
}



