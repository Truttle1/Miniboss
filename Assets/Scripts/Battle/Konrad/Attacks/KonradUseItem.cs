
using System.Collections;
using UnityEngine;

public class KonradUseItem : BattleAttack
{
    private Animator animator;

    [SerializeField] private GameObject useItemEyeCandy;

    private GameObject monster = null;

    public void setMonster(GameObject m)
    {
        if (m != null && m.GetComponent<Monster>() != null)
        {
            monster = m;
        }
        else
        {
            Debug.LogError("Provided GameObject is not a valid Monster.");
        }
    }
    void Start()
    {
        running = false;
        animator = GetComponent<Animator>();

    }

    private void createEyeCandy(Item item)
    {
        GameObject eyeCandy = Instantiate(useItemEyeCandy, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        eyeCandy.GetComponent<SpriteRenderer>().sprite = item.sprite;
    }

    private IEnumerator useItem(Item item)
    {
        running = true;
        // Start using item animation
        animator.SetBool("item", true);
        createEyeCandy(item);
        yield return new WaitForSeconds(1.4f);
        animator.SetBool("item", false);
        
        if(item.GetItemEffect().effectType == ItemEffectType.Heal)
        {
            // Reset running state
            yield return new WaitForSeconds(.6f);
            running = false;
            // Heal Konrad
            HasHP konradHP = GetComponent<HasHP>();
            konradHP.heal(item.GetItemEffect().value);
        }
        else if(item.GetItemEffect().effectType == ItemEffectType.SetStatusKonrad)
        {
            // Apply status effect to Konrad
            Konrad konrad = GetComponent<Konrad>();
            konrad.SetStatusEffect(item.GetItemEffect().konradStatusEffect, item.GetItemEffect().value);

            if(item.GetItemEffect().konradStatusEffect == KonradStatusEffect.Caffeinated)
            {
                konrad.addSelections(1);
            }
            
            // Reset running state
            yield return new WaitForSeconds(.6f);
            running = false;
        }
        else if(item.GetItemEffect().effectType == ItemEffectType.Attack)
        {
            switch(item.GetItemEffect().attack)
            {
                case ItemAttack.THROW_TOMATO:
                    KonradTomatoThrow konradTomatoThrow = GetComponent<KonradTomatoThrow>();
                    konradTomatoThrow.monster = monster;
                    konradTomatoThrow.startAttack();
                    while(konradTomatoThrow.isRunning())
                    {
                        yield return null;
                    }
                    break;
            }
            running = false;
        }
    }
    public override void startAttack()
    {
        if (attackArg is Item)
        {
            Item item = (Item)attackArg;
            StartCoroutine(useItem(item));
        }
        else
        {
            Debug.LogError("Attack argument is not an Item.");
            running = false;
        }
    }
}