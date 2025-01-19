using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

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
}
