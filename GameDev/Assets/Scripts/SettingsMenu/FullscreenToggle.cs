using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    void Start()
    {
        GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("Fullscreen", 1) != 0;
    }
}
