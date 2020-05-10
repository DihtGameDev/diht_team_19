using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Clickable : MonoBehaviour
{
    [Serializable]
    public class OnClickedEvent : UnityEvent {}
    [SerializeField]
    public OnClickedEvent OnClick = new OnClickedEvent();
}
