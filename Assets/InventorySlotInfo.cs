using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlotInfo
{
    public ItemScript ItemScript;
    public IntVector2 GridPos { get; private set; }
    public IntVector2 ItemStartPos { get; private set; }
    public SlotGrid SlotGrid { get; private set; }
    public SlotScript SlotScript;

    public InventorySlotInfo(IntVector2 gridPos, ItemScript itemObject, IntVector2 itemStartPos, SlotGrid slotGrid)
    {
        GridPos = gridPos;
        SlotGrid = slotGrid;
        ChangeItem(itemObject, itemStartPos);
    }

    public void ChangeItem(ItemScript itemScript, IntVector2 itemStartPos)
    {
        ItemScript = itemScript;
        ItemStartPos = itemStartPos;
    }
}
