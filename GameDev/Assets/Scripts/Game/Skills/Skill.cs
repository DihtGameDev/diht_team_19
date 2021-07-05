using System;
using System.Collections;
using System.Collections.Generic;
using Game.Animals;
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

        if (iteration == 1)
        {
            next.Add(aggression(1));
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

        if (iteration == 1)
        {
            next.Add(aggression(1));
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

    public static Skill armor(int iteration)
    {
        List<Skill> next;
        if (iteration > 3)
        {
            next = new List<Skill>();
        }
        else
        {
            next = new List<Skill> {armor(iteration + 1)};
        }

        return new Skill("Armor " + iteration, "Increase armor of all your creatures", 22 + iteration * 5, next,  (int player_id) =>
        {
            var player = GameController.Get().players[player_id];
            var health_delta = 5 + iteration * 2;
            player.data.health += health_delta;
            foreach (var pr in player.predators)
            {
                pr.health += health_delta;
            }
        });
    }
    public static Skill health(int iteration)
    {
        List<Skill> next;
        if (iteration > 5)
        {
            next = new List<Skill>();
        }
        else
        {
            next = new List<Skill> {health(iteration + 1)};
        }

        if (iteration == 3)
        {
            next.Add(armor(1));
        }
        
        return new Skill("Health " + iteration, "Increase health of all your creatures", 10 + iteration * 5, next,  (int player_id) =>
        {
            var player = GameController.Get().players[player_id];
            var health_delta = 5 + iteration * 2;
            player.data.health += health_delta;
            foreach (var pr in player.predators)
            {
                pr.health += health_delta;
            }
        });
    }
    
    public static readonly Skill foxUnique2 = new Skill("Fox unique 2", "Second unique fox skill", 0, new List<Skill>(), (int pid) => {});
    
    public static readonly Skill foxUnique = new Skill("Fox unique", "Unique fox skill", 0, new List<Skill>{foxUnique2}, (int pid) => {});

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
            var player = GameController.Get().players[player_id];
            foreach (var pr in player.predators)
            {
                
            }
            if (player.data.logging) Debug.Log("Added logic!");
        });
    }

    public static Skill roarAbility(int iteration)
    {
        List<Skill> next;
        if (iteration > 3)
        {
            next = new List<Skill>();
        }
        else
        {
            next = new List<Skill> {roarAbility(iteration + 1)};
        }
        
        return new Skill("Roar " + iteration, "Adds \"roar\" ability to your predators", 20 + iteration*5, next,
            (int pid) =>
            {
                var player = GameController.Get().players[pid];
                var roar = new Roar(15 * iteration, 5 + iteration * 3, 15 + iteration * 2);
                for (int i = 0; i < player.data.battleAbilities.Count; ++i)
                {
                    if (player.data.battleAbilities[i].GetName() != "Roar") continue;
                    player.data.battleAbilities.RemoveAt(i);
                    break;
                }
                player.data.battleAbilities.Add(roar);
                foreach (var pr in player.predators)
                {
                    for (int i = 0; i < pr.battleAbilities.Count; ++i)
                    {
                        if (pr.battleAbilities[i].GetName() == "Roar")
                        {
                            pr.battleAbilities.RemoveAt(i);
                            break;
                        }
                    }
                    pr.battleAbilities.Add(roar);
                }
            });
    } 
    public static Skill aggression(int iteration)
    {
        List<Skill> next;
        if (iteration > 3)
        {
            next = new List<Skill>();
        }
        else
        {
            next = new List<Skill> {aggression(iteration + 1)};
        }

        if (iteration == 1)
        {
            next.Add(roarAbility(1));
        }
        
        return new Skill("Aggression " + iteration, "Increase aggression towards other animals", 12 + 5*iteration, next,  (int player_id) =>
        {
            var player = GameController.Get().players[player_id];
            player.data.aggression += 4 + iteration;
            foreach (var pr in player.predators)
            {
                pr.aggression += 4 + iteration;
            }
            if (player.data.logging) Debug.Log("Added logic!");
        });
    }
    
    public static readonly Skill initial = new Skill("", "", 0, new List<Skill> {speed(1), hunger(1), health(1)}, (int pid) => { });
    
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



