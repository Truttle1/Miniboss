using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEditor.UI;
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

    [SerializeField]
    private AnimatorOverrideController normalOverride;

    [SerializeField]
    private AnimatorOverrideController poisonOverride;

    [SerializeField]
    private AnimatorOverrideController caffeinatedOverride;

    [SerializeField]
    private AnimatorOverrideController relaxedOverride;

    [SerializeField]
    private AnimatorOverrideController unhingedOverride;

    private bool statusEffectNew = false;

    private void SetStatusOverrideController()
    {
        switch(statusEffect)
        {
            case KonradStatusEffect.None:
            animator.runtimeAnimatorController = normalOverride;
            break;
            case KonradStatusEffect.Poison:
            animator.runtimeAnimatorController = poisonOverride;
            break;
            case KonradStatusEffect.Caffeinated:
            animator.runtimeAnimatorController = caffeinatedOverride;
            break;
            case KonradStatusEffect.Relaxed:
            animator.runtimeAnimatorController = relaxedOverride;
            break;
            case KonradStatusEffect.Unhinged:
            animator.runtimeAnimatorController = unhingedOverride;
            break;
        }
    }
    public int attack = 1;

    private float attackMultiplier = 1;

    private KonradPunch punchAttack;
    private KonradToasterGun toasterAttack;
    private KonradUseItem useItem;

    private KonradBreathe breathe;
    
    private HasHP hp;

    private BattleAttack currentAttack;
    private bool canBlock;

    [SerializeField]
    private KonradStatusEffect statusEffect = KonradStatusEffect.None;

    private int numSelectionsPerTurn = 1;


    private enum MenuState
    {
        NONE,
        TOP,
        ATTACK,
        SELECT_MONSTER,
        ITEM,
        SELECT_MONSTER_ITEM,
    };

    private enum SelectedAttack
    {
        PUNCH,
        TOAST
    }

    private const int PUNCH_COST = 1;

    private const int TOAST_COST = 3;

    private MenuState menuState;
    private SelectedAttack selectedAttack;

    private bool gotInitialStats = false;

    private bool battleFinished = false;

    private Rigidbody2D rb;

    private int lastAttackCost = 1;

    private int statusEffectTurnsLeft = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        punchAttack = GetComponent<KonradPunch>();
        toasterAttack = GetComponent<KonradToasterGun>();
        useItem = GetComponent<KonradUseItem>();
        breathe = GetComponent<KonradBreathe>();

        hp = GetComponent<HasHP>();
        rb = GetComponent<Rigidbody2D>();

        EventBus.Subscribe<EnemyStartAttackEvent>(EnemyStartAttack);
        EventBus.Subscribe<MenuItemSelectEvent>(MenuItemSelect);
        currentAttack = null;
        menuState = MenuState.NONE;
        StartCoroutine(GetInitialStats());
    }

    public void SetStatusMultipliers()
    {
        if(statusEffect == KonradStatusEffect.Relaxed)
        {
            attackMultiplier = 0.5f;
            hp.setDefense(2);
        }
        else if(statusEffect == KonradStatusEffect.Unhinged)
        {
            attackMultiplier = 1.5f;
            hp.setDefense(0.5f);
        }
        else
        {
            attackMultiplier = 1.0f;
            hp.setDefense(1);
        }
    }

    public void SetStatusEffect(KonradStatusEffect effect, int turns)
    {
        if (statusEffect != effect)
        {
            statusEffect = effect;
            statusEffectNew = true;
            statusEffectTurnsLeft = turns;
            SetStatusOverrideController();
        }
    }

    public float GetAttack()
    {
        return attack * attackMultiplier;
    }

    public void BetweenTurns()
    {
        if(statusEffectNew)
        {
            switch(statusEffect)
            {
                case KonradStatusEffect.None:
                    EventBus.Publish(new StatusBarEnableEvent("Konrad is back to normal!"));
                    break;
                case KonradStatusEffect.Poison:
                    EventBus.Publish(new StatusBarEnableEvent("Konrad is SICK!\nHe will take damage at the end of every turn!"));
                    break;
                case KonradStatusEffect.Caffeinated:
                    EventBus.Publish(new StatusBarEnableEvent("Konrad is CAFFEINATED!\nHe can move twice per turn, but cannot BREATHE!"));
                    break;
                case KonradStatusEffect.Unhinged:
                    EventBus.Publish(new StatusBarEnableEvent("Konrad is UNHINGED!\nHe both deals and takes more damage, and cannot BREATHE!"));
                    break;
                case KonradStatusEffect.Relaxed:
                    EventBus.Publish(new StatusBarEnableEvent("Konrad is RELAXED!\nHe needs less Energy to attack, does less damage, and gets more Energy from BREATHE!"));
                    break;
            }
            statusEffectNew = false;
        }
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

    private bool waitingToStartNewTurn = false;
    private bool tookDamageFromPoison = false;
    private bool takingDamageFromPoison = false;

    private IEnumerator TakeDamageFromPoison()
    {
        takingDamageFromPoison = true;
        setHurtAnimation(true);
        hp.damage(1);
        yield return new WaitForSeconds(1.0f);
        setHurtAnimation(false);
        tookDamageFromPoison = true;
    }

    private void Update()
    {
        if (takingTurn)
        {
            if (currentAttack != null)   //We have started attacking
            {
                if (!currentAttack.isRunning()) //...and the attack is no longer running
                {
                    if(!takingDamageFromPoison)
                    {
                        numSelectionsPerTurn -= 1;
                    }
                    if(numSelectionsPerTurn == 0)
                    {
                        if(statusEffect == KonradStatusEffect.Poison && !tookDamageFromPoison)
                        {
                            if(!takingDamageFromPoison)
                            {
                                StartCoroutine(TakeDamageFromPoison());
                            }
                        }
                        else
                        {
                            tookDamageFromPoison = false; // Reset poison damage flag
                            takingDamageFromPoison = false; // Reset poison damage coroutine flag
                            takingTurn = false; // So we are no longer taking our turn!
                            currentAttack = null; // No longer attacking
                        }
                    }
                    else
                    {
                        currentAttack = null; // No longer attacking
                        waitingToStartNewTurn = true;
                    }
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

        if(waitingToStartNewTurn)
        {
            if(!StatusBarController.instance.isRunning())
            {
                waitingToStartNewTurn = false;
                startTurn();
            }
        }

        SetStatusOverrideController();
        SetStatusMultipliers();
    }

    public void DisableBlock()
    {
        canBlock = false;
    }

    private void startTurn()
    {

        canBlock = false;
        if (BattleManager.instance.battleWon())
        {
            disableMenu();
        }
        else
        {
            SetMenuTop();
        }
    }

    protected override void takeTurnImpl()
    {
        if(statusEffectTurnsLeft > 0)
        {
            statusEffectTurnsLeft--;
            if(statusEffectTurnsLeft <= 0)
            {
                SetStatusEffect(KonradStatusEffect.None, 0);
            }
        }

        startTurn();

        if(statusEffect == KonradStatusEffect.Caffeinated)
        {
            numSelectionsPerTurn = 2;
        }
        else
        {
            numSelectionsPerTurn = 1;
        }
        //currentAttack = punchAttack;
        //currentAttack.startAttack();
    }

    public void addSelections(int selections)
    {
        numSelectionsPerTurn += selections;
    }

    private void spendAttackCost(int cost)
    {
        hp.damage(cost);
        lastAttackCost = cost;
        if(statusEffect == KonradStatusEffect.Relaxed)
        {
            lastAttackCost *= 4;
        }
    }

    private void startAttack(GameObject target)
    {
        if(selectedAttack == SelectedAttack.PUNCH)
        {
            currentAttack = punchAttack;
            ((KonradPunch)currentAttack).monster = target;
            spendAttackCost(PUNCH_COST);
        }

        if(selectedAttack == SelectedAttack.TOAST)
        {
            currentAttack = toasterAttack;
            ((KonradToasterGun)currentAttack).monster = target;
            spendAttackCost(TOAST_COST);
        }
        currentAttack.startAttack();
        disableMenu(); //Disable menu
    }

    private void doItem(string itemName, GameObject target = null)
    {
        Item item = GameManager.Instance.GetItemFromName(itemName);
        if (item != null && item.count > 0)
        {
            item.UseItem();
            currentAttack = useItem;
            currentAttack.setAttackArg(item);
            currentAttack.startAttack();
            if(item.GetItemEffect().effectType == ItemEffectType.Attack)
            {
                ((KonradUseItem)currentAttack).setMonster(target);
            }
            lastAttackCost = 1;
            disableMenu(); //Disable menu
        }
    }

    private void doBreathe()
    {
        currentAttack = breathe;
        currentAttack.setAttackArg(lastAttackCost);
        currentAttack.startAttack();
        lastAttackCost = 1;
        disableMenu(); //Disable menu
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
                case MenuState.SELECT_MONSTER_ITEM:
                    SelectMenuMonsterItem(message);
                    break;
            }
        }
    }

    private bool canBreathe()
    {
        if(statusEffect == KonradStatusEffect.Caffeinated || statusEffect == KonradStatusEffect.Unhinged)
        {
            return false;
        }
        return true;
    }
    private void SetMenuTop()
    {
        menuState = MenuState.TOP;
        EventBus.Publish(new MenuItemDataEvent(new[] {
            new MenuItemData("Attack", "", "ATTACK"),
            new MenuItemData("Item", "", "ITEM"),
            new MenuItemData("Breathe", "", canBreathe() ? "BREATHE" : "", "", canBreathe()),
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
                doBreathe();
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

    private string pendingItem = null;

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
                    Item itm = GameManager.Instance.GetItemFromName(message);
                    if(itm.GetItemEffect().effectType == ItemEffectType.Attack)
                    {
                        pendingItem = message;
                        SetMenuMonster(true);
                    }
                    else
                    {
                        doItem(message);
                    }
                }
                else
                {
                    Debug.LogWarning("Tried to use item that does not exist: " + message);
                }
                break;
        }
    }

    
    private void SelectMenuMonsterItem(string message)
    {
        switch (message)
        {
            case "BACK":
                SetMenuItem();
                break;
            default:
                GameObject target = BattleManager.instance.getMonsterFromName(message);
                if(target != null)
                {
                    doItem(pendingItem, target);
                }
                break;
        }
    }

    private bool HasEnoughEnergy(int cost)
    {
        return hp.getHP() > cost;
    }

    private int GetModifiedCost(int cost)
    {
        if(statusEffect == KonradStatusEffect.Relaxed)
        {
            return (int)Math.Floor(0.5f * cost);
        }
        if(statusEffect == KonradStatusEffect.Unhinged)
        {
            return (int)Math.Floor(1.5f * cost);
        }
        return Math.Max(0, cost);
    }

    private Color? GetMenuAttackColorFromStatus()
    {
        if(statusEffect == KonradStatusEffect.Relaxed)
        {
            return new Color32(10, 20, 115, 255);
        }
        if(statusEffect == KonradStatusEffect.Unhinged)
        {
            return new Color32(155, 10, 20, 255);
        }
        return null;
    }

    private void SetMenuAttack()
    {
        menuState = MenuState.ATTACK;
        EventBus.Publish(new MenuItemDataEvent(new[] {
            new MenuItemData(
                "Punch", 
                GetModifiedCost(PUNCH_COST) + "E", 
                "PUNCH", 
                HasEnoughEnergy(GetModifiedCost(PUNCH_COST)),
                GetMenuAttackColorFromStatus()
            ),

            new MenuItemData(
                "Toaster Gun",
                GetModifiedCost(TOAST_COST) + "E",
                "TOAST",
                HasEnoughEnergy(GetModifiedCost(TOAST_COST)),
                GetMenuAttackColorFromStatus()
            ),
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
            case "TOAST":
                selectedAttack = SelectedAttack.TOAST;
                SetMenuMonster();
                break;

        }
    }
    private void SetMenuMonster(bool items = false)
    {
        menuState = items ? MenuState.SELECT_MONSTER_ITEM : MenuState.SELECT_MONSTER;
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
        disableMenu(); // Disable menu
        StartCoroutine(FleeCoroutine());
    }

    private void disableMenu()
    {
        EventBus.Publish(new MenuItemDataEvent(false));
    }
}

public enum ActionCommand
{
    TRAFFIC_LIGHT,
    DAMGE_BAR,
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

public class ActionCommandNumericalFinishEvent
{
    public int value;
    public ActionCommandNumericalFinishEvent(int value)
    {
        this.value = value;
    }
}

public class EnemyStartAttackEvent
{
    public EnemyStartAttackEvent() {}
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