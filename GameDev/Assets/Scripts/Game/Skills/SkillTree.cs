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
        available.Clear();
        UpdateAvailable(number);
    }

    private void UpdateAvailable(int number)
    {
        // Scan through available and swap all null values for reachable,
        // But not available skills
        
        var updatedAvailable = new List<Skill>(Enumerable.Repeat<Skill>(null, number));
        var substitutions = GetReachableSkills();
        foreach (var skill in available)
        {
            substitutions.Remove(skill);
        }

        var indexes = Enumerable.Range(0, substitutions.Count).ToList();
        var rnd = GameController.rnd;
        indexes = indexes.OrderBy(x => rnd.Next()).ToList();
        
        var cur = 0;
        for (int i = 0; i < number; ++i)
        {
            if (cur == substitutions.Count && i >= available.Count) break;
            if (i < available.Count && available[i] != null)
            {
                updatedAvailable[i] = available[i];
            }
            else
            {
                if (cur == substitutions.Count) continue;
                updatedAvailable[i] = substitutions[indexes[cur]];
                Debug.Log(cur + " -> " + indexes[cur]);
                ++cur;
            }
        }

        available = updatedAvailable;
    }
    
    public void Open(Skill skill)
    {
        reachable.Add(skill);
    }
    
    private List<Skill> GetReachableSkills()
    {
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
    // Available are those that are available for purchase at UPGRADE menu. Subset of reachable
    private List<Skill> available = new List<Skill>();
}
