using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    private string currentText;
    
    private Queue<DialogPair> dialogPairs;
    private DialogPair currentPair = null;
    private int position = 0;
    private bool visible;
    private bool disableMovement;

    public Text text;
    
    public static TextBox instance;

    public AudioClip scrollSound;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        visible = false;
        dialogPairs = new Queue<DialogPair>();
        EventBus.Subscribe<DialogPair>(getDialog);
    }

    private void getDialog(DialogPair pair)
    {
        Debug.Log("Hello?");
        dialogPairs.Enqueue(pair);
    }

    private string ReplaceBackslashWithNewline(string input)
    {
        return input.Replace("\\", "\n");
    }

    private IEnumerator scrollText()
    {
        while(currentText != currentPair.text)
        {
            position += 1;
            EventBus.Publish(new PlaySFXEvent(scrollSound));
            if(position < currentPair.text.Length)
            {
                currentText = currentPair.text.Substring(0, position);
            }
            else
            {
                currentText = currentPair.text;
            }
            text.text = currentText;
            yield return new WaitForSeconds(0.04f);
        }
    }

    private IEnumerator reenableMovement()
    {
        yield return new WaitForSeconds(0.1f);
        disableMovement = false;
    }

    void Update()
    {
        if(visible)
        {
            gameObject.GetComponent<Image>().enabled = true;
            text.enabled = true;
        }
        else
        {
            gameObject.GetComponent<Image>().enabled = false;
            text.enabled = false;
        }

        if(dialogPairs.Count > 0 && currentPair == null)
        {
            currentPair = dialogPairs.Dequeue();
            currentPair.text = ReplaceBackslashWithNewline(currentPair.text);
            currentText = "";
            position = 0;
            StartCoroutine(scrollText());
            visible = true;
            disableMovement = true;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (visible)
            {
                if(currentText != currentPair.text)
                {
                    currentText = currentPair.text;
                    text.text = currentText;
                }
                else
                {
                    currentPair = null;
                    if(dialogPairs.Count == 0)
                    {
                        visible = false;
                        StartCoroutine(reenableMovement());
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && false)
        {
            EventBus.Publish(new DialogPair(null, "test string"));
            EventBus.Publish(new DialogPair(null, "another test string"));
        }
    }

    public bool showingText()
    {
        return visible;
    }

    public bool disablingMovement()
    {
        return disableMovement;
    }

    public Talker getTalker()
    {
        if(currentPair == null)
        {
            return null;
        }
        return currentPair.talker;
    }
}

public class DialogPair
{
    public Talker talker;
    public string text;

    public DialogPair(Talker talker, string text)
    {
        this.talker = talker;
        this.text = text;
    }
}