
using System.Collections;
using UnityEngine;

public class KonradBreathe : BattleAttack
{
    private Animator animator;

    void Start()
    {
        running = false;
        animator = GetComponent<Animator>();

    }

    private IEnumerator breathe(int amount)
    {
        running = true;
        // Start using item animation
        animator.SetBool("breathe", true);
        yield return new WaitForSeconds(1f);

        HasHP konradHP = GetComponent<HasHP>();
        konradHP.heal(amount);

        yield return new WaitForSeconds(2f);
        animator.SetBool("breathe", false);
        
        // Reset running state
        running = false;
    }
    public override void startAttack()
    {
        if (attackArg is int)
        {
            int amount = (int)attackArg;
            StartCoroutine(breathe(amount));
        }
        else
        {
            Debug.LogError("Attack argument is not an int.");
            running = false;
        }
    }
}