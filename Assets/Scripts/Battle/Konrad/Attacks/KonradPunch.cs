using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonradPunch : BattleAttack
{
    public GameObject monster;
    private Animator animator;
    private float xSpeed = 5f;
    private Vector3 walkVelocity;
    private Vector3 start;
    private Rigidbody2D rb;

    private bool waiting;
    private bool success;

    private Konrad konrad;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        walkVelocity = new Vector3(xSpeed, 0.0f, 0.0f);
        waiting = false;
        EventBus.Subscribe< ActionCommandFinishEvent>(actionCommandFinish);
        konrad = GetComponent<Konrad>();
    }

    private void actionCommandFinish(ActionCommandFinishEvent e)
    {
        if (waiting)
        {
            waiting = false;
            success = e.success;
        }
    }

    private IEnumerator punch()
    {
        start = transform.position;
        // Walk to monster
        animator.SetBool("walk", true);
        while(transform.position.x + 1.0f < monster.transform.position.x)
        {
            rb.MovePosition(transform.position + walkVelocity * Time.fixedDeltaTime);
            yield return null;
        }
        animator.SetBool("walk", false);

        if (monster.GetComponent<Monster>() != null)
        {
            monster.GetComponent<Monster>().setTarget(true);
        }

        // Punch
        animator.SetBool("start_punch", true);
        waiting = true;
        EventBus.Publish(new ActionCommandStartEvent(ActionCommand.TRAFFIC_LIGHT));
        while(waiting)
        {
            yield return null;
        }
        animator.SetBool("start_punch", false);

        if(success)
        {
            animator.SetBool("punch_good", true);
        }
        else
        {
            animator.SetBool("punch_bad", true);
        }
        yield return new WaitForSeconds((13 / 60.0f));

        int dmg;
        if(success)
        {
            dmg = monster.GetComponent<HasHP>().damage((int)(GetComponent<Konrad>().GetAttack() * 2));
        }
        else
        { 
            dmg = monster.GetComponent<HasHP>().damage((int)(GetComponent<Konrad>().GetAttack() * 1));
        }

        if(monster.GetComponent<Knockback>() != null)
        {
            monster.GetComponent<Knockback>().doKnockback(1.5f, .3f, dmg);
        }

        yield return new WaitForSeconds((27 / 60.0f));
        animator.SetBool("punch_good", false);
        animator.SetBool("punch_bad", false);

        // Walk back
        yield return new WaitForSeconds(.2f);
        animator.SetBool("walk", true);
        if (monster.GetComponent<Monster>() != null)
        {
            monster.GetComponent<Monster>().setTarget(false);
        }
        while (transform.position.x  > start.x)
        {
            transform.localScale = new Vector2(1.25f, 1.25f);
            rb.MovePosition(transform.position - walkVelocity * Time.fixedDeltaTime);
            yield return null;
        }
        transform.localScale = new Vector2(-1.25f, 1.25f);
        animator.SetBool("walk", false);
        rb.MovePosition(start);
        running = false;
        konrad.attackOver();

    }

    public override void startAttack()
    {
        running = true;
        StartCoroutine(punch());
    }
}
