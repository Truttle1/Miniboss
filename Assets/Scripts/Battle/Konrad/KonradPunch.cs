using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonradPunch : MonoBehaviour
{
    public GameObject monster;
    private Animator animator;
    private float xSpeed = 5f;
    private Vector3 walkVelocity;
    private Vector3 start;
    private Rigidbody2D rb;

    private bool waiting;
    private bool success;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        walkVelocity = new Vector3(xSpeed, 0.0f, 0.0f);
        waiting = false;
        EventBus.Subscribe< ActionCommandFinishEvent>(actionCommandFinish);
        StartCoroutine(punch());
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

        if(monster.GetComponent<Knockback>() != null)
        {
            monster.GetComponent<Knockback>().doKnockback(1.5f, .3f);
        }

        if(success)
        {
            monster.GetComponent<HasHP>().damage(GetComponent<Konrad>().attack * 2);
        }
        else
        { 
            monster.GetComponent<HasHP>().damage(GetComponent<Konrad>().attack * 1);
        }
        yield return new WaitForSeconds((27 / 60.0f));
        animator.SetBool("punch_good", false);
        animator.SetBool("punch_bad", false);

        // Walk back
        yield return new WaitForSeconds(.2f);
        animator.SetBool("walk", true);
        while (transform.position.x  > start.x)
        {
            transform.localScale = new Vector2(1.25f, 1.25f);
            rb.MovePosition(transform.position - walkVelocity * Time.fixedDeltaTime);
            yield return null;
        }
        transform.localScale = new Vector2(-1.25f, 1.25f);
        animator.SetBool("walk", false);
        rb.MovePosition(start);


    }
    void Update()
    {
        
    }
}
