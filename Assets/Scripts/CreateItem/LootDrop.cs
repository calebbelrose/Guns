using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Loot drop
[Serializable]
public class LootDrop
{
    public ItemClass ItemClass { get { return itemClass; } }
    public int Weight { get { return weight; } }

    [SerializeField] private ItemClass itemClass;
    [SerializeField] private int weight;

    public LootDrop(ItemClass itemClass, int weight)
    {
        this.itemClass = itemClass;
        this.weight = weight;
    }
}

//Common loot drop
public class CommonLootDrop : LootDrop
{
    public int MinAmount { get; private set; }
    public int MaxAmount { get; private set; }

    public CommonLootDrop(ItemClass itemClass, int weight, int minAmount, int maxAmount) : base(itemClass, weight)
    {
        MinAmount = minAmount;
        MaxAmount = maxAmount;
    }
}