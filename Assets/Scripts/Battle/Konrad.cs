using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Konrad : MonoBehaviour
{
    public int attack = 1; 
}

public enum ActionCommand
{
    TRAFFIC_LIGHT
}
public class ActionCommandStartEvent
{
    public ActionCommand command;
    public ActionCommandStartEvent(ActionCommand command)
    {
        this.command = command;
    }
}

public class ActionCommandFinishEvent
{
    public bool success;
    public ActionCommandFinishEvent(bool success)
    {
        this.success = success;
    }
}