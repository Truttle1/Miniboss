
using System.Collections;
using UnityEngine;

public class HideAndShowAtDistance : MonoBehaviour
{
    private GameObject konrad;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float showDistance = 3f;

    private bool visible = false;

    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            if(distance < showDistance)
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

}