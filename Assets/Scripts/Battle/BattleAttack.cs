using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleAttack : MonoBehaviour
{
    protected bool running;
    public abstract void startAttack();
    public bool isRunning()
    {
        return running;
    }
}
