using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartSlotInfo
{
    public bool Empty { get; private set; } = true;
    public ItemClass Item { get; private set; }
    public ItemClass ParentItem;
    public string Name;
    public List<int> PartIDs;
    public bool Required;
    public SlotScript Slot;

    public void FillSlot(ItemClass item)
    {
        Item = item;
        Empty = false;
    }

    public void ClearSlot()
    {
        Item = null;
        Empty = true;
    }
}
