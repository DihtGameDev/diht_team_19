using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Displayable : MonoBehaviour
{
    public abstract void AfterClick();

    public abstract string GetTitle();

    public abstract string GetInfo();

    public abstract string GetStatus();
}
