using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillTree
{
    public List<Skill> GetAvailableSkills(int number)
    {
        var list = GetReachableSkills();

        var answer = new List<Skill>();
        for (int i = 0; i < Math.Min(number, list.Count); ++i)
        {
            answer.Add(list[i]);   
        }
        
        return answer;
    }

    private List<Skill> GetReachableSkills()
    {
        return reachable.ToList();
    }

    public void Activate(Skill skill, int pid)
    {
        var new_reachable = new HashSet<Skill>(skill.GetNext());
        activated.Add(skill);
        reachable.UnionWith(new_reachable);
        reachable.ExceptWith(activated);
        skill.activate(pid);
    }
    
    private List<Skill> activated = new List<Skill> {Skill.initial};
    private HashSet<Skill> reachable = new HashSet<Skill>(Skill.initial.GetNext());
}
