using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHP : MonoBehaviour
{
    public int HP;
    public int maxHP;
    private bool blocking;
    public int getHP()
    {
        return HP;
    }

    public int getMaxHP()
    {
        return maxHP;
    }
    public bool isDead()
    {
        return HP <= 0;
    }

    public void setBlocking(bool blocking)
    {
        this.blocking = blocking;
    }

    public void damage(int damage)
    {
        if (blocking)
        {
            damage -= 1;
        }
        if (damage < 0)
        {
            damage = 0;
        }
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

    public bool isBlocking()
    {
        return blocking;
    }

    public void setMaxHP(int maxHP)
    {
        this.maxHP = maxHP;
        if (HP > maxHP)
        {
            HP = maxHP;
        }
    }

    public void setHP(int HP)
    {
        this.HP = HP;
        if (this.HP > maxHP)
        {
            this.HP = maxHP;
        }
        if (this.HP < 0)
        {
            this.HP = 0;
        }
    }
}
