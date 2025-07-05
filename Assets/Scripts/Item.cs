using System;
using UnityEngine;

[Serializable]
public enum ItemEffectType
{
    Heal,
    Attack,
    SetStatusKonrad,
}
[Serializable]
public enum ItemAttack
{
    NONE,
    THROW_TOMATO
}
[Serializable]
public class ItemEffect
{
    public ItemEffectType effectType;
    public int value;
    public KonradStatusEffect konradStatusEffect;

    public ItemAttack attack;
}


[Serializable]
public class Item
{
    public string name;
    public string description;
    public int count;

    public Sprite sprite;
    public ItemEffect itemEffect;

    public string GetName()
    {
        return name;
    }

    public string GetDescription()
    {
        return description;
    }

    public int GetCount()
    {
        return count;
    }

    public void AddCount(int amount)
    {
        count += amount;
        if (count < 0)
        {
            count = 0;
        }
    }

    public ItemEffect GetItemEffect()
    {
        return itemEffect;
    }


    /** 
     * UseItem method decreases the count of the item by 1 if there are items available.
     * Returns true if the item was successfully used, false otherwise.
     */
    public bool UseItem()
    {
        if (count > 0)
        {
            count--;
            return true;
        }
        return false;
    }

    public static explicit operator Item(UnityEngine.Object v)
    {
        throw new NotImplementedException();
    }
}