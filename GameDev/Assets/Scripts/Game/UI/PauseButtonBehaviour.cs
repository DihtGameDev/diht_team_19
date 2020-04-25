using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonBehaviour : MonoBehaviour
{
    public GameController controller;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("Pause was clicked!");
        if (paused)
        {
            controller.Play();
        } else
        {
            controller.Pause();
        }
        paused = !paused;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private bool paused = false;
}
