using System.Collections;
using System.Collections.Generic;
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

    private MenuState menuState;
    private void Start()
    {
        animator = GetComponent<Animator>();
        punchAttack = GetComponent<KonradPunch>();
        hp = GetComponent<HasHP>();
        EventBus.Subscribe<EnemyStartAttackEvent>(EnemyStartAttack);
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

    public void disableBlock()
    {
        canBlock = false;
    }

    protected override void takeTurnImpl()
    {
        canBlock = false;
        SetMenuTop();
        //currentAttack = punchAttack;
        //currentAttack.startAttack();
    }

    private void SetMenuTop()
    {
        menuState = MenuState.TOP;
        EventBus.Publish(new[] {
            new MenuItemData("Attack", "", "ATTACK"),
            new MenuItemData("Item", "", "ITEM"),
            new MenuItemData("Breathe", "", "BREATHE"),
        });
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