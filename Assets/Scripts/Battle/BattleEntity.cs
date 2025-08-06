using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEntity : MonoBehaviour
{
    protected Animator animator;
    protected bool takingTurn;
    
    [SerializeField] private BangSpawnPoint bangSpawnPoint;
    
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

    

    public void SpawnBang(int damage)
    {
        if (bangSpawnPoint != null)
        {
            bangSpawnPoint.SpawnBang(damage);
        }
        else
        {
            Debug.LogWarning("BangSpawnPoint is not assigned in the inspector.");
        }
    }

}
