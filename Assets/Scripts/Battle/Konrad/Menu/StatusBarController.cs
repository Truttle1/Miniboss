
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarController : MonoBehaviour
{
    [SerializeField]
    private GameObject container;

    [SerializeField]
    private TMP_Text text;

    private string textString;

    private bool running;

    private Coroutine closeCoroutine;

    public static StatusBarController instance;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        container.SetActive(false);
        running = false;
        EventBus.Subscribe<StatusBarEnableEvent>(SetStatusText);
    }

    void SetStatusText(StatusBarEnableEvent e)
    {
        textString = e.text;
        container.SetActive(true);
        running = true;
    }

    private IEnumerator closeAndWait()
    {
        container.SetActive(false);
        yield return new WaitForSeconds(0.33f);
        running = false;
        closeCoroutine = null;
    }


    void Update()
    {
        if(running)
        {
            if (textString != null && textString != "")
            {
                text.text = textString;
            }

            if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                if(closeCoroutine == null)
                {
                    StartCoroutine(closeAndWait());
                }
            }
        }
    }

    public bool isRunning()
    {
        return running;
    }


}

public class StatusBarEnableEvent
{
    public string text;
    public StatusBarEnableEvent(string t)
    {
        text = t;
    }
}