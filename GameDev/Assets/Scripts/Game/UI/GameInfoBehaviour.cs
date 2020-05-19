using TMPro;
using UnityEngine;

public class GameInfoBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI alive;

    [SerializeField] private TextMeshProUGUI points;

    public void UpdateAliveCount(int val)
    {
        alive.SetText(val.ToString());
    }

    public void UpdatePointsCount(int val)
    {
        points.SetText(val.ToString());
    }
}