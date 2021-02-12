using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlotInfo
{
    public ItemClass Item { get; private set; }
    public IntVector2 GridPos { get; private set; }
    public IntVector2 ItemStartPos { get; private set; }
    public SlotGrid SlotGrid { get; private set; }
    public bool Empty { get; private set; } = true;
    public SlotScript SlotScript;

    private static IntVector2 defaultPos = new IntVector2(-1, -1);

    public InventorySlotInfo(IntVector2 gridPos, SlotGrid slotGrid)
    {
        GridPos = gridPos;
        SlotGrid = slotGrid;
        RemoveItem();
    }

    public void ChangeItem(ItemClass item, IntVector2 itemStartPos)
    {
        Item = item;
        ItemStartPos = itemStartPos;
        Empty = false;
    }

    public void RemoveItem()
    {
        Item = null;
        ItemStartPos = defaultPos;
        Empty = true;
    }
}
