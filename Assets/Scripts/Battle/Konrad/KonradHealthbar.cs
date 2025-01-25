using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KonradHealthbar : MonoBehaviour
{
    public GameObject healthBar;
    public TMP_Text text;
    private float healthBarMaxSize;
    void Start()
    {
        healthBarMaxSize = healthBar.transform.localScale.x;
        EventBus.Subscribe<KonradHPChangeEvent>(hpChange);
    }

    private void hpChange(KonradHPChangeEvent evt)
    {
        text.SetText(evt.HP.ToString() + "/" +  evt.maxHP.ToString());
        healthBar.transform.localScale = new Vector3(((evt.HP * 1.0f) / evt.maxHP)*healthBarMaxSize, healthBar.transform.localScale.y, 1);
    }
}
