using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillTree
{
    public List<Skill> GetAvailableSkills(int number)
    {
        UpdateAvailable(number);

        return available;
    }
    
    public void ForceUpdateAvailable(int number)
    {
        var list = GetReachableSkills();

        available.Clear();
        for (int i = 0; i < Math.Min(number, list.Count); ++i)
        {
            available.Add(list[i]);   
        }
    }

    private void UpdateAvailable(int number)
    {
        var list = GetReachableSkills();
        foreach (var skill in available)
        {
            list.Remove(skill);
        }

        var cur = 0;
        for (int i = 0; i < available.Count; ++i)
        {
            if (cur == list.Count) break;
            if (available[i] != null) continue;
            available[i] = list[cur];
            cur++;
        }
    }
    
    public void Open(Skill skill)
    {
        reachable.Add(skill);
    }
    
    private List<Skill> GetReachableSkills()
    {
        Debug.Log(reachable);
        return reachable.ToList();
    }

    public void Activate(Skill skill, int pid)
    {
        var new_reachable = new HashSet<Skill>(skill.GetNext());
        Debug.Log(new_reachable.ToString());
        activated.Add(skill);
        reachable.UnionWith(new_reachable);
        reachable.ExceptWith(activated);
        skill.activate(pid);

        available.Remove(skill);
    }
    
    private List<Skill> activated = new List<Skill> {Skill.initial};
    private HashSet<Skill> reachable = new HashSet<Skill>(Skill.initial.GetNext());
    private List<Skill> available = new List<Skill>();
}
