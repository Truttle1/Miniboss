using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMovement : MonoBehaviour
{

    private Transform player_position;
    private float x_diff;
    private float y_diff;
    private bool moving = true;

    [SerializeField] private float x_min = 0.0f;
    [SerializeField] private float x_max = 0.0f;

    [SerializeField] private float y_min = 0.0f;
    [SerializeField] private float y_max = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(GameObject.Find("Konrad"))
        {
            player_position = GameObject.Find("Konrad").transform;
        }
        if (player_position == null)
        {
            return;
        }
        x_diff = transform.position.x - player_position.position.x;
        y_diff = transform.position.y - player_position.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(player_position == null && GameObject.Find("Konrad"))
        {
            player_position = GameObject.Find("Konrad").transform;
            return;
        }

        if (moving && player_position != null)
        {
            float new_x = player_position.position.x + x_diff;
            float new_y = player_position.position.y + y_diff;

            if (new_x < x_min)
            {
                new_x = x_min;
            }

            if (new_y < y_min)
            {
                new_y = y_min;
            }

            if (new_x > x_max)
            {
                new_x = x_max;
            }
            if (new_y > y_max)
            {
                new_y = y_max;
            }
            transform.position = new Vector3(new_x, new_y, -10);
        }
    }
}
