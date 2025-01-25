using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEntity : MonoBehaviour
{
    protected Animator animator;
    protected bool takingTurn;
    
    public void setHurtAnimation(bool hurt)
    {
        if (hurt)
        {
            animator.SetBool("hurt", true);
        }
        else
        {
            animator.SetBool("hurt", false);
        }
    }

    protected abstract void takeTurnImpl();
    public void takeTurn()
    {
        takingTurn = true;
        takeTurnImpl();
    }

    public bool isTakingTurn()
    {
        return takingTurn;
    }

}
