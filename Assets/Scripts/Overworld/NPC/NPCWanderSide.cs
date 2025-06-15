using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCWanderSideDirection
{
    Left,
    Right
}
public class NPCWanderSide : MonoBehaviour
{
    public NPCWanderSideDirection direction;

    private NPCWander wander;
    // Start is called before the first frame update
    void Start()
    {
        wander = transform.parent.GetComponent<NPCWander>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            wander.SideCollision(direction);
        }
    }
}
