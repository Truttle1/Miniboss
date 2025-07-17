using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TalkableNPC : MonoBehaviour
{
    public List<Talker> dialogTalkers;
    public List<string> dialogText;
    public float range = 2.0f;
    public bool facePlayer = true;
    public bool normallyFacesRight = true;

    private Talker talker;
    private Animator animator;
    private Transform playerPos;

    void Start()
    {
        talker = GetComponent<Talker>();
        animator = GetComponent<Animator>();
        if(GameObject.Find("Konrad"))
        {
            playerPos = GameObject.Find("Konrad").transform;
        }
    }

    void Update()
    {
        if (playerPos == null)
        {
            if(GameObject.Find("Konrad"))
            {
                playerPos = GameObject.Find("Konrad").transform;
            }
            else
            {
                return;
            }
        }

        if (!TextBox.instance.showingText())
        {
            float distance = Vector2.Distance(new Vector2(playerPos.position.x, playerPos.position.y), new Vector2(transform.position.x, transform.position.y));
            if(distance < range)
            {
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    for(int i = 0; i <  dialogTalkers.Count; i++)
                    {
                        DialogPair pair = new DialogPair(dialogTalkers[i], dialogText[i]);
                        EventBus.Publish(pair);
                    }
                }
            }
        }

        if (facePlayer)
        {
            if (playerPos.position.x < gameObject.transform.position.x)
            {
                if (normallyFacesRight)
                {
                    transform.localScale = new Vector2(-1, 1);
                }
                else
                {
                    transform.localScale = new Vector2(1, 1);
                }
            }
            else
            {
                if (normallyFacesRight)
                {
                    transform.localScale = new Vector2(1, 1);
                }
                else
                {
                    transform.localScale = new Vector2(-1, 1);
                }
            }
        }

        animator.SetBool("talk", TextBox.instance.getTalker() != null && TextBox.instance.getTalker() == talker);
    }
}
