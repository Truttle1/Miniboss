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

    public void SetState(string left, string right, string msg)
    {
        highlight.SetActive(false);
        message = msg;

        leftText.SetText(left);
        rightText.SetText(right);
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
