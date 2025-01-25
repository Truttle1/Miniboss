using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleEntity[] entities;
    private BattleEntity currentEntity;
    private int currentEntityID;
    void Start()
    {
        currentEntity = null;
        currentEntityID = -1;
    }

    void Update()
    {
        if(currentEntity == null || !currentEntity.isTakingTurn())
        {
            getNextEntity();
        }
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
            if (entities[i] is Monster)
            {
                monsters.Add((Monster) entities[i]);
            }
        }
        return monsters.ToArray();
    }
}
