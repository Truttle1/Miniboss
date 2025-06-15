using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWander : MonoBehaviour
{
    // Start is called before the first frame update
    public float[] toggleTimes;
    public float duration;

    public BoxCollider2D leftSideCollider;
    public BoxCollider2D rightSideCollider;

    public float maxVelocity = 2.0f;

    private Rigidbody2D rb;
    private float velocityX;
    private bool moving = true;
    private float mostRecentToggleTime = 999f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        velocityX = maxVelocity;
        StartCoroutine(WanderCoroutine());
    }

    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        velocityX = maxVelocity;
        StartCoroutine(WanderCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector3(moving ? velocityX : 0, rb.velocity.y, 0);
    }

    private IEnumerator WanderCoroutine()
    {
        float timer = 0.0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                timer = 0.0f;
                mostRecentToggleTime = 999f;
            }


            for(int i = 0; i < toggleTimes.Length; i++)
            {
                float absToggleTime = Math.Abs(toggleTimes[i]);
                if(Math.Abs(timer - absToggleTime) < 0.1f && mostRecentToggleTime != toggleTimes[i])
                {
                    mostRecentToggleTime = toggleTimes[i];
                    bool flip = toggleTimes[i] < 0;
                    if(flip)
                    {
                        velocityX = -velocityX;
                    }
                    else
                    {
                        moving = !moving;
                    }
                }
            }

            yield return null;
        }
    }

    public void SideCollision(NPCWanderSideDirection direction) 
    {
        if (direction == NPCWanderSideDirection.Left)
        {
            if(velocityX < 0) 
            {
                velocityX = maxVelocity;
            }
        }
        else if (direction == NPCWanderSideDirection.Right)
        {
            if(velocityX > 0) 
            {
                velocityX = -maxVelocity;
            }
        }
    }
}
