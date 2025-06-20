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
    private Konrad konrad;

    private bool startedEndText;

    private bool canLeaveBattle = false;



    [SerializeField] private float fadeTime = 0.5f; // Duration of the fade effect
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
        startedEndText = false;
        canLeaveBattle = false;

        
        GameObject encounterPrefab = GameManager.Instance.getEncounter();
        GameObject encounterInstance = Instantiate(encounterPrefab);

        encounterInstance.name = "Encounter";
        List<BattleEntity> allEntities = new List<BattleEntity>(entities);

        allEntities.AddRange(encounterInstance.GetComponentsInChildren<Monster>());
        entities = allEntities.ToArray();

        konrad = FindObjectOfType<Konrad>();
        EventBus.Publish(new FadeEvent(true));
        EventBus.Subscribe<PermitLeaveBattleEvent>(OnPermitLeaveBattle);
    }

    public bool battleWon()
    {
        return getMonsters().Length == 0 && konrad != null && konrad.isBattleFinished() && !konrad.isDead();
    }
    
    public bool allEnemiesDead()
    {
        return getMonsters().Length == 0;
    }

    void Update()
    {
        if(currentEntity == null || !currentEntity.isTakingTurn())
        {
            getNextEntity();
        }

        if(!startedEndText)
        {
            if(battleWon())
            {
                startedEndText = true;
                EventBus.Publish(new BigTextStartEvent(BigTextType.Win));
            }
            else if(konrad != null && konrad.isDead())
            {
                startedEndText = true;
                EventBus.Publish(new BigTextStartEvent(BigTextType.GameOver));
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if(canLeaveBattle)
            {
                StartCoroutine(endEncounter());
            }
        }

    }

    private IEnumerator endEncounter()
    {
        canLeaveBattle = false;
        OverworldManager overworld = FindObjectOfType<OverworldManager>().GetComponent<OverworldManager>();
        if(battleWon())
        {
            GameManager.Instance.setBattleStatus(LastBattleStatus.Victory);
        }
        
        if(overworld != null)
        {
            EventBus.Publish(new FadeEvent(false));
            yield return new WaitForSeconds(0.5f);
            overworld.OnEncounterEnd();
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

    private void OnPermitLeaveBattle(PermitLeaveBattleEvent leaveEvent)
    {
        canLeaveBattle = true;
    }
}

public class PermitLeaveBattleEvent
{
    public PermitLeaveBattleEvent()
    {
    }
}
