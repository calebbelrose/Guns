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
    public SlotScript SlotScript;

    public InventorySlotInfo(IntVector2 gridPos, ItemClass item, IntVector2 itemStartPos, SlotGrid slotGrid)
    {
        GridPos = gridPos;
        SlotGrid = slotGrid;
        ChangeItem(item, itemStartPos);
    }

    public void ChangeItem(ItemClass item, IntVector2 itemStartPos)
    {
        Item = item;
        ItemStartPos = itemStartPos;
    }
}
