using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
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

    private int battleEXP = 0;

    private int battleMoney = 0;




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
        konrad.BetweenTurns();
        if(currentEntity == null || !currentEntity.isTakingTurn())
        {
            if(StatusBarController.instance.isRunning())
            {
                return;
            }
            getNextEntity();
        }

        if(!startedEndText)
        {
            if(battleWon())
            {
                startedEndText = true;
                int oldLevel = GameManager.Instance.getLevel();
                GameManager.Instance.addEXP(battleEXP);
                bool levelUp = GameManager.Instance.getLevel() > oldLevel;
                
                EventBus.Publish(new BigTextStartEvent(BigTextType.Win, battleEXP, battleMoney, levelUp));
                battleEXP = 0;
                battleMoney = 0;
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
                if(entities[i].gameObject.GetComponent<HasHP>() && 
                   entities[i].gameObject.GetComponent<HasHP>().getHP() <= 0)
                {
                    continue; // Skip dead monsters
                }
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

    public void addExp(int expToAdd)
    {
        battleEXP += expToAdd;
    }

    public void addMoney(int moneyToAdd)
    {
        battleMoney += moneyToAdd;
    }
}

public class PermitLeaveBattleEvent
{
    public PermitLeaveBattleEvent()
    {
    }
}
