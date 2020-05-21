using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTreeUI : MonoBehaviour
{
    [SerializeField] private List<SkillButton> alternatives;

    public void PrepareUI()
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
    }
}
