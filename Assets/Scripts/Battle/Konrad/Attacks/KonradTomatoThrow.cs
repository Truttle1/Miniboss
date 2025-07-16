using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonradTomatoThrow : BattleAttack
{
    public GameObject monster;
    public GameObject projectile;
    private Animator animator;
    private Konrad konrad;
    public int damageMultiplier = 4;

    void Start()
    {
        animator = GetComponent<Animator>();
        konrad = GetComponent<Konrad>();
    }

    public IEnumerator attack()
    {
        if (monster.GetComponent<Monster>() != null)
        {
            monster.GetComponent<Monster>().setTarget(true);
        }

        animator.SetBool("throw", true);
        yield return new WaitForSeconds(15f / 24f);

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
        int dmg = monster.GetComponent<HasHP>().damage((int)(GetComponent<Konrad>().GetAttack() * damageMultiplier));
        if(monster.GetComponent<Knockback>() != null)
        {
            monster.GetComponent<Knockback>().doKnockback(1.5f, knockbackTime, dmg);
        }
        yield return new WaitForSeconds(knockbackTime);

        animator.SetBool("throw", false);

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
