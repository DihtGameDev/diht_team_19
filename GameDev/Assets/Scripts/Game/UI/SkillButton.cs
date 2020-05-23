using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerClickHandler
{
   
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;
    [SerializeField] private SkillTreeUI ui;
    
    private Skill binded = null;
    private string description;

    public void bind(Skill skill)
    {
        binded = skill;
        if (binded != null)
        {
            buttonText.SetText(skill.GetName());
            description = skill.GetDescription();
        }
        else
        {
            buttonText.SetText("No skills available");
            description = "No skills available";
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (binded != null)
            {
                GameController.Get().players[0].tree.Activate(binded, 0);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log(description);
        }
        ui.UpdateUI();
    }
}
