using System;
using TMPro;
using UnityEngine;

public class SidePanelBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI title;
    
    [SerializeField]
    private TextMeshProUGUI status;
    
    [SerializeField]
    private TextMeshProUGUI satiety;

    private PredatorController predator_;
    public void Bind(PredatorController predator)
    {
        predator_ = predator;
    }

    void Update()
    {
        if (predator_ != null)
        {
            status.SetText("State: " + GetStateString(predator_.GetState()));
            title.SetText(predator_.GetName());
            satiety.SetText("Satiety: " + predator_.GetSatiety().ToString("0"));
        }      
    }

    private string GetStateString(AnimalState state)
    {
        switch (state)
        {
            case AnimalState.Calm:
                return "Calm";
                break;
            case AnimalState.Hungry:
                return "Hungry";
                break;
            case AnimalState.Afraid:
                return "Afraid";
                break;
            case AnimalState.Frenzy:
                return "Frenzy";
                break;
            case AnimalState.Dead:
                return "Dead";
                break;
            case AnimalState.Overate:
                return "Overate";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }   
    }
}
