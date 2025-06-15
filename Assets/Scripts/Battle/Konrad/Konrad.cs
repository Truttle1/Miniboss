using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEditor.VersionControl;
using UnityEngine;

public enum KonradStatusEffect
{
    None,
    Poison,
    Caffeinated,
    Relaxed,
    Unhinged,
}

public class Konrad : BattleEntity
{
    public int attack = 1;

    private KonradPunch punchAttack;
    private KonradUseItem useItem;
    private HasHP hp;

    private BattleAttack currentAttack;
    private bool canBlock;

    private KonradStatusEffect statusEffect = KonradStatusEffect.None;


    private enum MenuState
    {
        NONE,
        TOP,
        ATTACK,
        SELECT_MONSTER,
        ITEM
    };

    private enum SelectedAttack
    {
        PUNCH
    }

    private const int PUNCH_COST = 1;

    private MenuState menuState;
    private SelectedAttack selectedAttack;

    private bool gotInitialStats = false;

    private bool battleFinished = false;

    private Rigidbody2D rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        punchAttack = GetComponent<KonradPunch>();
        useItem = GetComponent<KonradUseItem>();

        hp = GetComponent<HasHP>();
        rb = GetComponent<Rigidbody2D>();

        EventBus.Subscribe<EnemyStartAttackEvent>(EnemyStartAttack);
        EventBus.Subscribe<MenuItemSelectEvent>(MenuItemSelect);
        currentAttack = null;
        menuState = MenuState.NONE;
        StartCoroutine(GetInitialStats());

    }

    private IEnumerator GetInitialStats()
    {
        do
        {
            yield return null;
        }
        while(GameManager.Instance == null);

        hp.setMaxHP(GameManager.Instance.getMaxHP());
        hp.setHP(GameManager.Instance.getCurrentHP());
        gotInitialStats = true;
    }

    private void EnemyStartAttack(EnemyStartAttackEvent e)
    {
        canBlock = true;
    }

    private IEnumerator konradBlock()
    {
        canBlock = false;

        animator.SetBool("block", true);
        yield return new WaitForSeconds(10.0f / 60);
        hp.setBlocking(true);
        yield return new WaitForSeconds(40.0f / 60);
        hp.setBlocking(false);
        animator.SetBool("block", false);
    }

    private void Update()
    {
        if (takingTurn)
        {
            if (currentAttack != null)   //We have started attacking
            {
                if (!currentAttack.isRunning()) //...and the attack is no longer running
                {
                    takingTurn = false; // So we are no longer taking our turn!
                    currentAttack = null; // No longer attacking
                }
            }
        }
        else
        {
            if (canBlock)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                {
                    StartCoroutine(konradBlock());
                }
            }
        }

        EventBus.Publish(new KonradHPChangeEvent(hp.getHP(), hp.getMaxHP()));

        if(gotInitialStats)
        {
            GameManager.Instance.setHPStats(hp.getMaxHP(), hp.getHP());
        }
    }

    public void DisableBlock()
    {
        canBlock = false;
    }

    protected override void takeTurnImpl()
    {
        canBlock = false;
        if (BattleManager.instance.battleWon())
        {
            EventBus.Publish(new MenuItemDataEvent(false));
        }
        else
        {
            SetMenuTop();
        }
        //currentAttack = punchAttack;
        //currentAttack.startAttack();
    }

    private void startAttack(GameObject target)
    {
        if(selectedAttack == SelectedAttack.PUNCH)
        {
            currentAttack = punchAttack;
            ((KonradPunch)currentAttack).monster = target;
            hp.damage(PUNCH_COST);
        }
        currentAttack.startAttack();
        EventBus.Publish(new MenuItemDataEvent(false)); //Disable menu
    }

    private IEnumerator useItemCoroutine(Item item)
    {
        animator.SetBool("item", true);

        yield return new WaitForSeconds(4.0f);

        animator.SetBool("item", false);
        attackOver();
    }

    private void doItem(string itemName)
    {
        Item item = GameManager.Instance.GetItemFromName(itemName);
        if (item != null && item.count > 0)
        {
            item.UseItem();
            currentAttack = useItem;
            currentAttack.setAttackArg(item);
            currentAttack.startAttack();
            EventBus.Publish(new MenuItemDataEvent(false)); //Disable menu
        }
    }

    /* Menu Stuff */
    private void MenuItemSelect(MenuItemSelectEvent e)
    {
        string message = e.message;
        if (message != null)
        {
            switch (menuState)
            {
                case MenuState.TOP:
                    SelectMenuTop(message);
                    break;
                case MenuState.ITEM:
                    SelectMenuItem(message);
                    break;
                case MenuState.ATTACK:
                    SelectMenuAttack(message);
                    break;
                case MenuState.SELECT_MONSTER:
                    SelectMenuMonster(message);
                    break;
            }
        }
    }
    private void SetMenuTop()
    {
        menuState = MenuState.TOP;
        EventBus.Publish(new MenuItemDataEvent(new[] {
            new MenuItemData("Attack", "", "ATTACK"),
            new MenuItemData("Item", "", "ITEM"),
            new MenuItemData("Breathe", "", "BREATHE"),
            new MenuItemData("Escape", "", "ESCAPE"),
        }, false));
    }

    private void SelectMenuTop(string message)
    {
        switch (message)
        {
            case "BACK":
                break;
            case "ATTACK":
                SetMenuAttack();
                break;
            case "BREATHE":
                break;
            case "ITEM":
                SetMenuItem();
                break;
            case "ESCAPE":
                Flee();
                break;
        }
    }

    private void SetMenuItem()
    {
        menuState = MenuState.ITEM;
        List<MenuItemData> items = new List<MenuItemData>();
        for(int i = 0; i < GameManager.Instance.getItemList().Length; i++)
        {
            Item item = GameManager.Instance.getItemList()[i];
            if(item.count > 0)
            {
                items.Add(
                    new MenuItemData(item.GetName(), 
                    "x" + item.GetCount().ToString(), 
                    item.GetName(),
                    item.GetDescription())
                );
            }
        }
        EventBus.Publish(new MenuItemDataEvent(items.ToArray(), true));
    }

    private void SelectMenuItem(string message)
    {

        switch (message)
        {
            case "BACK":
                SetMenuTop();
                break;
            default:
                if (GameManager.Instance.GetItemFromName(message) != null)
                {
                    doItem(message);
                }
                else
                {
                    Debug.LogWarning("Tried to use item that does not exist: " + message);
                }
                break;
        }
    }

    private void SetMenuAttack()
    {
        menuState = MenuState.ATTACK;
        EventBus.Publish(new MenuItemDataEvent(new[] {
            new MenuItemData("Punch", "1E", "PUNCH"),
        }, true));
    }
    private void SelectMenuAttack(string message)
    {
        switch (message)
        {
            case "BACK":
                SetMenuTop();
                break;
            case "PUNCH":
                selectedAttack = SelectedAttack.PUNCH;
                SetMenuMonster();
                break;
        }
    }
    private void SetMenuMonster()
    {
        menuState = MenuState.SELECT_MONSTER;
        List<MenuItemData> monsterMenuItems = new List<MenuItemData>();
        foreach(Monster monster in BattleManager.instance.getMonsters())
        {
            monsterMenuItems.Add(new MenuItemData(monster.getMenuName(), "", monster.getMenuName()));
        }
        EventBus.Publish(new MenuItemDataEvent(monsterMenuItems.ToArray(), true));
    }
    private void SelectMenuMonster(string message)
    {
        switch (message)
        {
            case "BACK":
                SetMenuAttack();
                break;
            default:
                GameObject target = BattleManager.instance.getMonsterFromName(message);
                if(target != null)
                {
                    startAttack(target);
                }
                break;
        }
    }

    public void attackOver() 
    {
        if(BattleManager.instance.allEnemiesDead())
        {
            battleFinished = true;
            animator.SetBool("win", true);
        }
    }

    public bool isBattleFinished()
    {
        return battleFinished;
    }

    public bool isDead()
    {
        return hp.getHP() <= 0;
    }

    public IEnumerator FleeCoroutine()
    {
        transform.localScale = new Vector2(1.25f, 1.25f);
        animator.SetBool("flee", true);
        float startTime = Time.time;
        while(Time.time < startTime + 1.0f) 
        {
            rb.MovePosition(transform.position - new Vector3(3.0f, 0, 0) * Time.fixedDeltaTime);
            yield return null;
        }
        EventBus.Publish(new BigTextStartEvent(BigTextType.Escape));
        GameManager.Instance.setBattleStatus(LastBattleStatus.Escape);
        while(true) 
        {
            rb.MovePosition(transform.position - new Vector3(3.0f, 0, 0) * Time.fixedDeltaTime);
            yield return null;
        }
    }
    
    private void Flee()
    {
        EventBus.Publish(new MenuItemDataEvent(false)); // Disable menu
        StartCoroutine(FleeCoroutine());
    }
}

public enum ActionCommand
{
    TRAFFIC_LIGHT
}
public class ActionCommandStartEvent
{
    public ActionCommand command;
    public ActionCommandStartEvent(ActionCommand command)
    {
        this.command = command;
    }
}

public class ActionCommandFinishEvent
{
    public bool success;
    public ActionCommandFinishEvent(bool success)
    {
        this.success = success;
    }
}

public class EnemyStartAttackEvent
{
    public EnemyStartAttackEvent()
    {

    }
}

public class KonradHPChangeEvent
{
    public int HP;
    public int maxHP;
    public KonradHPChangeEvent(int hP, int maxHP)
    {
        HP = hP;
        this.maxHP = maxHP;
    }
}