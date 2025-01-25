using UnityEngine;


public class Monster : BattleEntity
{
    public int attack = 1;
    public BattleAttack[] attacks;
    private MonsterHealthbar healthbar;
    private BattleAttack currentAttack;
    private bool targeted = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        healthbar = GetComponent<MonsterHealthbar>();
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

        healthbar.setHP(GetComponent<HasHP>().HP, GetComponent<HasHP>().maxHP);
    }
    protected override void takeTurnImpl()
    {
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
}
