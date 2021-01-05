using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Ammo")]
public class Ammo : Pickupable
{
    public AmmoType AmmoType;
    public int Damage;
    public int Penetration;

    public override void AddToInventory(Inventory inventory, GameObject gameObject, int amount)
    {
        inventory.AddToInventory(gameObject, AmmoType, amount);
    }

    public override string GetName()
    {
        return GetEnumDescription(AmmoType);
    }
}

public enum AmmoType
{
    [Description("9 Millimeter")]
    Ninemill,
    [Description(".456")]
    Fourfivesix,
    [Description(".50 Cal")]
    Fiftycal,
    [Description("Slugs")]
    Slugs
}