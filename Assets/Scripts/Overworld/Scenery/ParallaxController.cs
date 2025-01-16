using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private GameObject background;

    [SerializeField]
    private int backgroundCount;

    [SerializeField]
    private float yOffset;

    private float[] xOffset;
    private GameObject[] bgObjects;
    private const float BG_WIDTH = 12.8f;
    void Start()
    {
        float lastOffset = -BG_WIDTH/2.0f;
        xOffset = new float[backgroundCount];
        bgObjects = new GameObject[backgroundCount];
        for (int i = 0; i < backgroundCount; i++)
        {
            xOffset[i] = lastOffset;
            bgObjects[i] = Instantiate(background, new Vector3(0, 0, 0), Quaternion.identity);
            lastOffset += BG_WIDTH;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < backgroundCount; i++)
        {
            bgObjects[i].transform.position = new Vector3(.75f * Camera.main.transform.position.x + xOffset[i], yOffset, 2);
        }
    }
}
