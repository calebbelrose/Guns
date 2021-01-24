using System;
using UnityEngine;

[Serializable]
public class EquipSlotInfo
{
    public ItemScript ItemScript;
    public EquipSlot EquipSlot;

    public CategoryName CategoryName { get { return categoryName; } }

    [SerializeField] private CategoryName categoryName;
}
