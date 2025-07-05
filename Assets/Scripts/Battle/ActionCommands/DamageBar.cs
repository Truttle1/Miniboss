using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DamageBar : MonoBehaviour
{

    public GameObject arrow;

    public SpriteRenderer successText;

    public Sprite stGreat;
    public Sprite stMeh;
    public Sprite stTerrible;

    public float leftmostRed = -10;
    public float rightmostRed = 4;

    public float leftmostYellow = -6.8f;
    public float rightmostYellow = .8f;

    public float leftmostGreen = -3.5f;
    public float rightmostGreen = -2.5f;

    public float speed = 0.4f;

    private float directionMultiplier = 1;

    private bool running;

    public float offsetX = 2.7f;

    void Start()
    {
        running = false;
        SendOffScreen();
        EventBus.Subscribe<ActionCommandStartEvent>(ActionCommandStart);
    }

    void ActionCommandStart(ActionCommandStartEvent e)
    {

        if(e.command == ActionCommand.DAMGE_BAR)
        {
            BringOntoScreen();
        }
    }


    private IEnumerator AfterCommand(int amount)
    {
        Sprite stSprite = stTerrible;
        switch(amount)
        {
            case 1:
                stSprite = stTerrible;
                break;
            case 2:
                stSprite = stMeh;
                break;
            case 3:
                stSprite = stGreat;
                break;
        }
        successText.enabled = true;
        successText.sprite = stSprite;

        running = false;
        yield return new WaitForSeconds(.7f);
        EventBus.Publish(new ActionCommandNumericalFinishEvent(amount));
        SendOffScreen();
    }

    void Update()
    {
        if (running)
        {
            float arrowX = arrow.gameObject.transform.position.x;
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                int amount = 1;
                if(arrowX > leftmostGreen + offsetX && arrowX < rightmostGreen + offsetX)
                {
                    amount = 3;
                }
                else if(arrowX > leftmostYellow  + offsetX && arrowX < rightmostYellow + offsetX)
                {
                    amount = 2;
                }
                StartCoroutine(AfterCommand(amount));
            }

            arrow.gameObject.transform.position = arrow.gameObject.transform.position + new Vector3(speed * directionMultiplier * Time.deltaTime, 0);
            if(arrowX > rightmostRed + offsetX)
            {
                directionMultiplier = -1;
            }

            if(arrowX < leftmostRed + offsetX)
            {
                directionMultiplier = 1;
            }
        }
    }

    private const float OFFSCREEN = 10.0f;

    private void BringOntoScreen()
    {
        running = true;
        successText.enabled = false;
        arrow.gameObject.transform.position = new Vector3(leftmostRed, arrow.gameObject.transform.position.y);
        directionMultiplier = 1;

        gameObject.transform.position = new Vector3(offsetX, 0, 0);
    }

    private void SendOffScreen()
    {
        running = false;
        gameObject.transform.position = new Vector3(0, OFFSCREEN, 0);
    }
}
