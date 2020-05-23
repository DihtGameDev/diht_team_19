using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<PredatorController> predators = new List<PredatorController>();
    public PredatorData data = new PredatorData();
    public int points = 30;
    public SkillTree tree = new SkillTree();
}
