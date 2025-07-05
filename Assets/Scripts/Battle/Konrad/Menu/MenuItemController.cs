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

    private bool itemEnabled = true;

    private Color? overrideColor = null;

    public void SetState(string left, string right, string msg, string flavorText, Color? overrideColor = null)
    {
        highlight.SetActive(false);
        message = msg;
        this.flavorText = flavorText;

        leftText.SetText(left);
        rightText.SetText(right);

        this.overrideColor = overrideColor;
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

        if(itemEnabled)
        {
            if(overrideColor != null)
            {
                leftText.color = overrideColor.Value;
                rightText.color = overrideColor.Value;
            }
            else
            {
                leftText.color = new Color32(65, 115, 0, 255);
                rightText.color = new Color32(65, 115, 0, 255);
            }
        }
        else
        {
            leftText.color = new Color32(100, 100, 100, 255);
            rightText.color = new Color32(100, 100, 100, 255);
        }
    }

    public void setSelected(bool selected)
    {
        this.selected = selected;
    }

    public void setItemEnabled(bool itemEnabled)
    {
        this.itemEnabled = itemEnabled;
    }
}
