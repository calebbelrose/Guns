using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    public StatName StatName { get { return statName; } }

    [SerializeField] private int baseValue;
    [SerializeField] private StatName statName;
    [SerializeField] private List<StatBonus> StatBonuses = new List<StatBonus>();
    [SerializeField] private List<StatMultiplier> StatMultipliers = new List<StatMultiplier>();

    public Stat(StatName name, int _baseValue)
    {
        statName = name;
        baseValue = _baseValue;
    }

    //Changes base stat
    public void ChangeStat(int value)
    {
        baseValue = value;
    }

    //Adds stat bonus
    public void AddStatBonus(StatBonus statBonus)
    {
        StatBonuses.Add(statBonus);
    }

    //Removes stat bonus
    public void RemoveStatBonus(StatBonus statBonus)
    {
        StatBonuses.Remove(StatBonuses.Find(x => x.BonusValue == statBonus.BonusValue));
    }

    //Returns the final value of the stat
    public int FinalValue
    {
        get
        {
            int finalValue = baseValue;
            StatBonuses.ForEach(x => finalValue += x.BonusValue);
            return finalValue;
        }
    }
}