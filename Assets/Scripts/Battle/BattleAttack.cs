using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleAttack : MonoBehaviour
{
    protected bool running;
    public abstract void startAttack();

    protected System.Object attackArg;
    public bool isRunning()
    {
        return running;
    }

    public void setAttackArg(System.Object arg)
    {
        attackArg = arg;
    }


}
