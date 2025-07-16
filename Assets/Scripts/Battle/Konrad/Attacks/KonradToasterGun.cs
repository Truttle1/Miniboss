using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonradToasterGun : BattleAttack
{
    public GameObject monster;
    public GameObject projectile;
    private Animator animator;
    private bool waiting;
    private int damage;

    private Konrad konrad;

    void Start()
    {
        animator = GetComponent<Animator>();
        waiting = false;
        EventBus.Subscribe< ActionCommandNumericalFinishEvent>(actionCommandFinish);
        konrad = GetComponent<Konrad>();
    }

    private void actionCommandFinish(ActionCommandNumericalFinishEvent e)
    {
        if (waiting)
        {
            waiting = false;
            damage = e.value;
        }
    }

    private IEnumerator attack()
    {
        if (monster.GetComponent<Monster>() != null)
        {
            monster.GetComponent<Monster>().setTarget(true);
        }

        animator.SetBool("toast_running", true);
        yield return new WaitForSeconds(1.8f);
        waiting = true;
        EventBus.Publish(new ActionCommandStartEvent(ActionCommand.DAMGE_BAR));
        while(waiting)
        {
            yield return null;
        }

        for(int i = 0; i < damage; i++) 
        {
            animator.SetBool("toast_launch", true);
            yield return new WaitForSeconds(4f / 24f);
            animator.SetBool("toast_launch", false);

            // Launch projectile
            GameObject p = Instantiate(projectile, transform.position + new Vector3(0.5f, 0.0f, 0.0f), Quaternion.identity);
            float duration = 0.4f;
            float knockbackTime = .3f;
            float goalTime = Time.time + duration;
            Vector3 startPos = p.transform.position;
            while(Time.time < goalTime)
            {
                float progress = 1f - (goalTime - Time.time) / duration;
                p.transform.position = Vector3.Lerp(startPos, monster.transform.position, progress);
                yield return null;
            }
            Destroy(p);
            int dmg = 
            monster.GetComponent<HasHP>().damage((int)(GetComponent<Konrad>().GetAttack() * 2));
            if(monster.GetComponent<Knockback>() != null)
            {
                monster.GetComponent<Knockback>().doKnockback(1.5f, knockbackTime, dmg);
            }
            yield return new WaitForSeconds(knockbackTime);

            // Check if monster is dead
            if(monster.GetComponent<HasHP>() != null && monster.GetComponent<HasHP>().getHP() <= 0)
            {
                break;
            }
        }
        animator.SetBool("toast_launch", false);
        
        animator.SetBool("toast_running", false);
        

        if (monster && monster.GetComponent<Monster>() != null)
        {
            monster.GetComponent<Monster>().setTarget(false);
        }
        yield return new WaitForSeconds(0.75f);
        running = false;
        konrad.attackOver();
    }

    public override void startAttack()
    {
        running = true;
        StartCoroutine(attack());
    }
}
