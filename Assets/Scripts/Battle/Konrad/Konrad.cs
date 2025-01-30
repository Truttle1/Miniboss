using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class Konrad : BattleEntity
{
    public int attack = 1;

    private KonradPunch punchAttack;
    private HasHP hp;

    private BattleAttack currentAttack;
    private bool canBlock;

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

    private MenuState menuState;
    private SelectedAttack selectedAttack;

    private void Start()
    {
        animator = GetComponent<Animator>();
        punchAttack = GetComponent<KonradPunch>();
        hp = GetComponent<HasHP>();
        EventBus.Subscribe<EnemyStartAttackEvent>(EnemyStartAttack);
        EventBus.Subscribe<MenuItemSelectEvent>(MenuItemSelect);
        currentAttack = null;
        menuState = MenuState.NONE;
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
            hp.damage(1);
        }
        currentAttack.startAttack();
        EventBus.Publish(new MenuItemDataEvent(false)); //Disable menu
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