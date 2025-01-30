using System.Collections;
using UnityEngine;


public class Monster : BattleEntity
{
    public string menuName;
    public int attack = 1;
    public BattleAttack[] attacks;
    public GameObject arrow;


    private MonsterHealthbar healthbar;
    private BattleAttack currentAttack;
    private bool targeted = false;
    private bool hoveredByMenu = false;
    private bool dead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        healthbar = GetComponent<MonsterHealthbar>();
        EventBus.Subscribe<MenuItemHoverEvent>(selectItem);
    }

    public string getMenuName()
    {
        return menuName;
    }

    private void Update()
    {

        if (!dead && takingTurn)
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

        healthbar.setHP(GetComponent<HasHP>().HP, GetComponent<HasHP>().maxHP);

        if(GetComponent<HasHP>().getHP() <= 0 && !dead)
        {
            dead = true;
            animator.SetBool("dead", true);
            StartCoroutine(die());
        }

        arrow.SetActive(hoveredByMenu);
    }

    private IEnumerator die()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
    public bool getHovered()
    {
        return hoveredByMenu;
    }
    protected override void takeTurnImpl()
    {
        if (dead)
        {
            return;
        }
        int index = Random.Range(0, attacks.Length);
        currentAttack = attacks[index];
        attacks[index].startAttack();
        EventBus.Publish(new EnemyStartAttackEvent());
    }

    public void setTarget(bool targeted)
    {
        this.targeted = targeted;
    }

    public bool getTarget()
    {
        return this.targeted;
    }

    private void selectItem(MenuItemHoverEvent e)
    {
        if (e.message == menuName || e.message == "ALL")
        {
            hoveredByMenu = true;
        }
        else
        {
            hoveredByMenu = false;
        }
    }
}
