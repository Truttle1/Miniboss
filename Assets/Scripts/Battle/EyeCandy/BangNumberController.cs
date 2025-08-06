using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BangNumberController : MonoBehaviour
{
    [SerializeField]
    private TextMesh text;

    [SerializeField]
    private int number = 0;
    public void SetNumber(int number)
    {
        this.number = number;
        text.text = number.ToString();
    }

    void Start()
    {
        text.gameObject.GetComponent<MeshRenderer>().sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
    }

    void Update()
    {
        text.text = number.ToString();
    }
}
