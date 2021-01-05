using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Tactical")]
public class Tactical : Pickupable
{
    public TacticalType TacticalType;

    public override void AddToInventory(Inventory inventory, GameObject gameObject, int amount)
    {
        inventory.AddToInventory(gameObject, TacticalType, amount);
    }

    public override string GetName()
    {
        return GetEnumDescription(TacticalType);
    }
}

public enum TacticalType
{
    [Description("Stun")]
    Stun,
    [Description("Flash")]
    Flash,
    [Description("Heartbeat")]
    Heartbeat
}