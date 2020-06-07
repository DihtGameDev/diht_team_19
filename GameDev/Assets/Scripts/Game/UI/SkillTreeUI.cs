using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTreeUI : MonoBehaviour
{
    [SerializeField] private List<SkillButton> alternatives;

    private void OnEnable()
    {
        UpdateUi();
    }

    public void UpdateUi()
    {
        var player = GameController.Get().players[0];
        var tree = player.tree;
        var skills = tree.GetAvailableSkills(alternatives.Count);
        
        for (int i = skills.Count; i < alternatives.Count; ++i)
        {
            skills.Add(null);
        }
        
        for (int i = 0; i < alternatives.Count; ++i)
        {
            alternatives[i].bind(skills[i]);
        }

        Debug.Log("Available:");
        foreach (var skill in skills)
        {
            if (skill != null)
            {
                Debug.Log(skill.GetName());
            }
            else
            {
                Debug.Log("None");
            }
            
        }
    }

    public void ForceUpdate()
    {
        var player = GameController.Get().players[0];
        var tree = player.tree;
        tree.ForceUpdateAvailable(alternatives.Count);
        UpdateUi();
    }
}
