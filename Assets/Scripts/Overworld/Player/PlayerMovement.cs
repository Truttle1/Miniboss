using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;

    private const float MAX_HORIZ_SPEED = 6.0f;
    private const float JUMP = 12.0f;

    private bool facingLeft = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * MAX_HORIZ_SPEED;
        float verticalMovement = rb.velocity.y;
        bool onGround = Physics2D.Raycast(transform.position, Vector2.down, boxCollider.bounds.extents.y + 0.1f, LayerMask.GetMask("Wall"));
        Debug.Log(onGround);

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if(onGround)
            {
                verticalMovement = JUMP;
            }
        }

        if(horizontalMovement > .1f)
        {
            facingLeft = false;
        }

        if(horizontalMovement < -.1f)
        {
            facingLeft = true;
        }

        if(facingLeft)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }
        rb.velocity = new Vector2(horizontalMovement, verticalMovement);

        animator.SetBool("run", Math.Abs(rb.velocity.x) > 0.001f);
        animator.SetBool("jump", rb.velocity.y > 1f);
        animator.SetBool("land", rb.velocity.y < -1f);
    }
}
