using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuItemController : MonoBehaviour
{
    public GameObject highlight;

    private bool selected;
    public TMP_Text leftText;
    public TMP_Text rightText;
    private string message;

    private string flavorText;

    public void SetState(string left, string right, string msg, string flavorText)
    {
        highlight.SetActive(false);
        message = msg;
        this.flavorText = flavorText;

        leftText.SetText(left);
        rightText.SetText(right);
    }

    public string getMessage()
    {
        return message;
    }

    public string getFlavorText()
    {
        return flavorText;
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            highlight.SetActive(true);
        }
        else
        {
            highlight.SetActive(false);
        }
    }

    public void setSelected(bool selected)
    {
        this.selected = selected;
    }
}
