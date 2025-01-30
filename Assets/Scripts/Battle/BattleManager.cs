using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public BattleEntity[] entities;
    public GameObject winText;

    private BattleEntity currentEntity;
    private int currentEntityID;
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        currentEntity = null;
        currentEntityID = -1;
    }

    public bool battleWon()
    {
        return getMonsters().Length == 0;
    }

    void Update()
    {
        if(currentEntity == null || !currentEntity.isTakingTurn())
        {
            getNextEntity();
        }

        winText.SetActive(battleWon());

    }

    void getNextEntity()
    {
        currentEntityID++;
        if(currentEntityID == entities.Length)
        {
            currentEntityID = 0;
        }
        currentEntity = entities[currentEntityID];
        currentEntity.takeTurn();
    }

    public Monster[] getMonsters()
    {
        List<Monster> monsters = new List<Monster>();
        for(int i = 0; i < entities.Length; i++)
        {
            if (entities[i] != null && entities[i] is Monster)
            {
                monsters.Add((Monster) entities[i]);
            }
        }
        return monsters.ToArray();
    }

    public GameObject getMonsterFromName(string name)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i] is Monster)
            {
                if (((Monster)entities[i]).getMenuName() == name)
                {
                    return entities[i].gameObject;
                }
            }
        }
        return null;
    }
}
