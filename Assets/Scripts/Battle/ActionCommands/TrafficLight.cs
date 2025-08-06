using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    private SpriteRenderer sr;

    public Sprite traffic0;
    public Sprite traffic1;
    public Sprite traffic2;
    public Sprite trafficEnd;
    public Sprite trafficSuccess;
    public Sprite trafficFailLate;
    public Sprite trafficFailEarly;

    private float delay = .5f;
    private bool running;
    private bool good;
    private bool success;
    private Coroutine coroutine;

    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip lightSound0;
    [SerializeField] private AudioClip lightSound1;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        EventBus.Subscribe<ActionCommandStartEvent>(ActionCommandStart);
    }

    void ActionCommandStart(ActionCommandStartEvent e)
    {

        if(e.command == ActionCommand.TRAFFIC_LIGHT)
        {
            coroutine = StartCoroutine(TrafficLightCommand());
        }
    }

    private IEnumerator TrafficLightCommand()
    {
        success = false;
        sr.enabled = true;
        running = true;
        good = false;
        sr.sprite = traffic0;
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(lightSound0, Camera.main.transform.position);
        sr.sprite = traffic1;
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(lightSound0, Camera.main.transform.position);
        sr.sprite = traffic2;
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(lightSound1, Camera.main.transform.position);
        good = true;
        sr.sprite = trafficEnd;
        yield return new WaitForSeconds(delay);
        good = false;
        sr.sprite = trafficFailLate;
        StartCoroutine(AfterCommand());
    }

    private IEnumerator AfterCommand()
    {
        if(success)
        {
            AudioSource.PlayClipAtPoint(successSound, Camera.main.transform.position);
        }
        else
        {
            AudioSource.PlayClipAtPoint(failSound, Camera.main.transform.position);
        }
        running = false;
        yield return new WaitForSeconds(.7f);
        sr.enabled = false;
        EventBus.Publish(new ActionCommandFinishEvent(success));
    }

    void Update()
    {
        if (running)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                if(good)
                {
                    sr.sprite = trafficSuccess;
                    success = true;
                }
                else
                {
                    sr.sprite = trafficFailEarly;
                    success = false;
                }
                StopCoroutine(coroutine);
                StartCoroutine(AfterCommand());
            }
        }
    }
}
