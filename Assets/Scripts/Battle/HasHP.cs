using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHP : MonoBehaviour
{
    public int HP;
    public int maxHP;

    public int getHP()
    {
        return HP;
    }

    public bool isDead()
    {
        return HP <= 0;
    }

    public void damage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
        }
    }

    public void heal(int heal)
    {
        HP += heal;
        if (HP > maxHP)
        {
            HP = maxHP;
        }
    }
}
