using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Lethal")]
public class Lethal : Pickupable
{
    public LethalType LethalType;

    public override void AddToInventory(Inventory inventory, GameObject gameObject, int amount)
    {
        inventory.AddToInventory(gameObject, LethalType, amount);
    }

    public override string GetName()
    {
        return GetEnumDescription(LethalType);
    }
}

public enum LethalType
{
    [Description("Grenade")]
    Grenade,
    [Description("Mine")]
    Mine,
    [Description("Throwing Knife")]
    ThrowingKnife
}