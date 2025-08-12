using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const float MAX_HORIZ_SPEED = 6.0f;
    private const float JUMP = 13.0f;
    private bool facingLeft = true;
    private bool iFrame = false;

    [SerializeField] private float iFrameDuration = 5f;
    [SerializeField] private float translucentAlpha = 0.5f;

    [SerializeField] private AudioClip jump;

    private bool autoMove = false;
    private float autoMoveSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private bool IsOnGround()
    {
        for(int i = 0; i < 100; i++)
        {
            Vector2 rayOrigin = new Vector2(boxCollider.bounds.min.x + ((i - 50) / 50f) * boxCollider.bounds.size.x, boxCollider.bounds.min.y);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.1f, LayerMask.GetMask("Wall"));
            if(hit.collider != null)
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * MAX_HORIZ_SPEED;
        float verticalMovement = rb.velocity.y;
        //bool onGround = Physics2D.Raycast(transform.position, Vector2.down, boxCollider.bounds.extents.y + 0.1f, LayerMask.GetMask("Wall"));
        bool onGround = IsOnGround();
        
        if(autoMove)
        {
            horizontalMovement = autoMoveSpeed;
        }
        else
        {
            if (TextBox.instance.disablingMovement())
            {
                rb.velocity = new Vector2(0, verticalMovement);
                animator.SetBool("run", false);
                animator.SetBool("jump", false);
                animator.SetBool("land", false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                if(onGround)
                {
                    verticalMovement = JUMP;
                    EventBus.Publish(new PlaySFXEvent(jump));
                }
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

        animator.SetBool("run", Math.Abs(rb.velocity.x) > 0.01f);
        animator.SetBool("jump", rb.velocity.y > 1f);
        animator.SetBool("land", rb.velocity.y < -1f);
    }

    
    public void TriggerIFrames()
    {
        if (!iFrame)
        {
            DeleteAllNamedEncounter();
            StartCoroutine(IFrameCoroutine());
        }
    }

    
    void DeleteAllNamedEncounter()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Encounter")
            {
                Destroy(obj);
            }
        }
    }

    private IEnumerator IFrameCoroutine()
    {
        iFrame = true;

        // Make player translucent
        Color c = spriteRenderer.color;
        c.a = translucentAlpha;
        spriteRenderer.color = c;

        // Ignore collisions with enemies
        Collider2D[] enemyColliders = GameObject.FindObjectsOfType<Collider2D>();
        int enemyLayerIndex = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayerIndex, true);

        yield return new WaitForSeconds(iFrameDuration);

        // Restore opacity
        c.a = 1f;
        spriteRenderer.color = c;

        // Re-enable collisions
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayerIndex, false);

        iFrame = false;
    }

    public void SetFacingLeft(bool facingLeft)
    {
        this.facingLeft = facingLeft;
        if (facingLeft)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    public void DisableAutoMove()
    {
        if (autoMove)
        {
            autoMove = false;
            autoMoveSpeed = 0f;
        }
    }

    public void EnableAutoMove(float speed)
    {
        autoMove = true;
        autoMoveSpeed = speed;
    }
}
