using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SidePanelBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI title;
    
    [SerializeField]
    private TextMeshProUGUI status;
    
    [SerializeField]
    private TextMeshProUGUI info;

    private Displayable active_;
    public void Bind(Displayable obj)
    {
        active_ = obj;
    }

    void Update()
    {
        if (active_ != null)
        {
            status.SetText(active_.GetStatus());
            title.SetText(active_.GetTitle());
            info.SetText(active_.GetInfo());
        }      
    }

    
}
