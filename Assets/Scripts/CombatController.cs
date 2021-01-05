﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatController : MonoBehaviour
{
    public float MagicFind { get; private set; } = 1f;
    public int Level { get; private set; } = 1;
    public Animator Animator { get { return animator; } }

    [SerializeField] protected List<EquipSlot> EquipSlots = new List<EquipSlot>();

    [SerializeField] private Animator animator;
    [SerializeField] private List<Stat> Stats = new List<Stat>();

    protected float CurrentHealth = 100, MaxHealth = 100;
    protected float CurrentResource = 100, MaxResource = 100;
    protected float experience = 0, experienceToNextLevel = 100;

    public static int MaxLevel { get; } = 100;

    //Adds stat bonus
    public void AddStatBonus(List<StatBonus> statBonuses)
    {
        foreach (StatBonus statBonus in statBonuses)
            Stats.Find(x => x.StatName == statBonus.StatName).AddStatBonus(statBonus);
    }

    //Removes stat bonus
    public void RemoveStatBonus(List<StatBonus> statBonuses)
    {
        foreach (StatBonus statBonus in statBonuses)
            Stats.Find(x => x.StatName == statBonus.StatName).RemoveStatBonus(statBonus);
    }

    //Performs attack
    public void PerformAttack()
    {
        Animator.SetTrigger("Attacking");
    }

    //Equips item
    public void Equip(ItemScript itemScript)
    {
        EquipSlot equipSlot = EquipSlots.Find(x => x.CategoryName == itemScript.item.CategoryName);
        EquipInSlot(itemScript, equipSlot);
    }

    //Equips item in slot
    public void EquipInSlot(ItemScript itemScript, EquipSlot equipSlot)
    {
        if (itemScript.item.CategoryName == equipSlot.CategoryName)
        {
            itemScript.transform.SetParent(equipSlot.transform);
            itemScript.transform.localPosition = Vector3.zero;
            itemScript.CanvasGroup.alpha = 1f;

            if (equipSlot.Empty)
                ItemScript.ResetSelectedItem();
            else
                ItemScript.SetSelectedItem(equipSlot.Item);

            equipSlot.Item = itemScript;
            equipSlot.Empty = false;
            equipSlot.Image.color = Color.white;
            equipSlot.EquipObject.SetActive(true);
        }
    }

    //Unequips item
    public void Unequip(EquipSlot equipSlot)
    {
        ItemScript.SetSelectedItem(equipSlot.Item);
        equipSlot.Empty = true;
        equipSlot.EquipObject.SetActive(false);
    }

    //Takes damage
    public virtual void TakeDamage(float amount, CombatController attacker)
    {
        if (amount > 0)
        {
            CurrentHealth -= amount;

            if (CurrentHealth <= 0)
            {
                attacker.AwardExperience(Level * MaxHealth);
                Destroy(gameObject);
            }
        }
    }

    //Uses resource
    public virtual void UseResource(float amount)
    {
        CurrentResource -= amount;
    }

    //Awards experience
    protected virtual void AwardExperience(float amount)
    {
        experience += amount;
        while (experience >= experienceToNextLevel)
        {
            experience -= experienceToNextLevel;
            experienceToNextLevel = (int)(experienceToNextLevel * 1.25);
            Level++;
        }
    }

    //Returns ordered stats
    public List<Stat> OrderedStats()
    {
        return Stats.OrderBy(x => UnityEngine.Random.value).Take(UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(StatName)).Length)).ToList();
    }

    //Returns crit chance
    public bool Crit
    {
        get
        {
            float Luck = Stats.Find(x => x.StatName.ToString() == "Luck").FinalValue;

            return Random.Range(0, Luck + 10000f) <= Luck;
        }
    }

    //Returns crit damage
    public float CritMultiplier
    {
        get
        {
            return 1 + Stats.Find(x => x.StatName.ToString() == "Dexterity").FinalValue / 1000f;
        }
    }

    //Returns strength multiplier
    public float StrengthMultiplier
    {
        get
        {
            return 1 + Stats.Find(x => x.StatName.ToString() == "Strength").FinalValue / 1000f;
        }
    }
}