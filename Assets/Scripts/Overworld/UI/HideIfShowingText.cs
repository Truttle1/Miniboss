
using UnityEngine;

public class HideIfShowingText : MonoBehaviour
{
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(TextBox.instance.showingText())
        {
            sr.enabled = false;
        }
        else
        {
            sr.enabled = true;
        }
    }
}