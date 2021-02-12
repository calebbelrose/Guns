using System;
using UnityEngine;

[Serializable]
public class EquipSlotInfo
{
    public ItemClass ItemClass;
    public EquipSlot EquipSlot;
    public bool Empty = true;

    [SerializeField] protected CategoryName categoryName;

    public virtual bool CanEquip(ItemClass item)
    {
        return item.CategoryName == categoryName;
    }
}
