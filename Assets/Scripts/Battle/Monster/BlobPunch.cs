using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class BlobPunch : BattleAttack
{
    private GameObject konrad;
    private Animator animator;
    private float xSpeed = 5f;
    private Vector3 walkVelocity;
    private Vector3 start;
    private Rigidbody2D rb;

    void Start()
    {
        konrad = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        walkVelocity = new Vector3(xSpeed, 0.0f, 0.0f);
    }
    private IEnumerator punch()
    {
        start = transform.position;
        transform.position = new Vector3(start.x, start.y, -7f);
        transform.localScale = new Vector2(1f, 1f);

        // Walk to monster
        animator.SetBool("moving", true);
        while (transform.position.x - 1.0f > konrad.transform.position.x)
        {
            rb.MovePosition(transform.position - walkVelocity * Time.fixedDeltaTime);
            yield return null;
        }
        animator.SetBool("moving", false);

        // Punch
        animator.SetBool("punching", true);
        yield return new WaitForSeconds((124 / 60.0f));
        int dmg = konrad.GetComponent<HasHP>().damage(GetComponent<Monster>().attack * 2);

        if (konrad.GetComponent<Knockback>() != null)
        {
            konrad.GetComponent<Knockback>().doKnockback(-1.5f, .3f, dmg);
        }

        konrad.GetComponent<Konrad>().DisableBlock();


        // Walk back
        yield return new WaitForSeconds(.7f);
        animator.SetBool("punching", false);
        animator.SetBool("moving", true);
        while (transform.position.x < start.x)
        {
            transform.localScale = new Vector2(-1f, 1f);
            rb.MovePosition(transform.position + walkVelocity * Time.fixedDeltaTime);
            yield return null;
        }
        transform.localScale = new Vector2(1f, 1f);
        animator.SetBool("moving", false);
        rb.MovePosition(start);
        transform.position = new Vector3(start.x, start.y, -1f);
        running = false;
    }

    public override void startAttack()
    {
        running = true;
        StartCoroutine(punch());
    }
}
