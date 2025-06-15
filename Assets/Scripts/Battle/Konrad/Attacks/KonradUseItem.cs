
using System.Collections;
using UnityEngine;

public class KonradUseItem : BattleAttack
{
    private Animator animator;

    [SerializeField] private GameObject useItemEyeCandy;
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
        yield return new WaitForSeconds(.6f);
        
        // Reset running state
        running = false;
        if(item.GetItemEffect().effectType == ItemEffectType.Heal)
        {
            // Heal Konrad
            HasHP konradHP = GetComponent<HasHP>();
            konradHP.heal(item.GetItemEffect().value);
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