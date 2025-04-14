using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Talker : MonoBehaviour
{
    private bool isTalking = false;
    public void setTalking(bool isTalking)
    {
        this.isTalking = isTalking;
    }

    public bool getTalking()
    {
        return isTalking;
    }
}