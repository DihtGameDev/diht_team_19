using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<PredatorController> predators = new List<PredatorController>();
    public PredatorData data = new PredatorData();
    public uint points = 0;
    public SkillTree tree = new SkillTree();
}
