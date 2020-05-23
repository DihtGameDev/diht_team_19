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
                var player = GameController.Get().players[0];
                
                if (player.points >= binded.GetPrice())
                {
                    Debug.Log(player.points);
                    Debug.Log(binded.GetPrice());
                    player.tree.Activate(binded, 0);
                    player.points -= binded.GetPrice();
                    GameController.Get().updatePointInfo();
                }
                else
                {
                    Debug.Log("Not enough points!");
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log(description);
        }
        ui.UpdateUI();
    }
}
