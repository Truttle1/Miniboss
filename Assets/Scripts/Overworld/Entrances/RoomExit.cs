using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomExit : MonoBehaviour
{
    [SerializeField] private string destinationRoom;
    [SerializeField] private string destinationLabel;
    [SerializeField] private Direction direction = Direction.Left;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private GameObject konrad;

    private bool visible = false;

    private bool triggered = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(direction == Direction.Right)
        {
            animator.SetBool("right", true);
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
    }

    private IEnumerator FadeIn()
    {
        float startTime = Time.time;
        float duration = 0.25f;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            spriteRenderer.color = new Color(1f, 1f, 1f, t);
            yield return null;
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    private IEnumerator FadeOut()
    {
        float startTime = Time.time;
        float duration = 0.25f;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            spriteRenderer.color = new Color(1f, 1f, 1f, 1 - t);
            yield return null;
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
    }

    void Update()
    {
        if(konrad == null)
        {
            konrad = GameObject.Find("Konrad");
        }
        else
        {
            float distance = Vector2.Distance(new Vector2(konrad.transform.position.x, konrad.transform.position.y), new Vector2(transform.position.x, transform.position.y));
            if(distance < 3f)
            {
                if(!visible)
                {
                    visible = true;
                    StartCoroutine(FadeIn());
                }
            }
            else
            {
                if(visible)
                {
                    visible = false;
                    StartCoroutine(FadeOut());
                }
            }
        }
    }

    private IEnumerator FadeToRoom()
    {
        EventBus.Publish(new FadeEvent(false));
        yield return new WaitForSeconds(1f);
        OverworldManager.Instance.SetEntranceLabel(destinationLabel);
        SceneManager.LoadScene(destinationRoom);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        if (collision.gameObject.name == "Konrad")
        {
            triggered = true;
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            playerMovement.EnableAutoMove(direction == Direction.Left ? -2f : 2f);
            StartCoroutine(FadeToRoom());
        }
    }
}
