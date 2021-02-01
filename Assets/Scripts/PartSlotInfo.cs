using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartSlotInfo
{
    public ItemClass Item;
    public ItemClass ParentItem;
    public string Name;
    public List<int> PartIDs;
    public bool Required;
    public SlotScript Slot;
}
